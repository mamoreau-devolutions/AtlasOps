namespace AtlasOps.Integrations.Tests;

using AtlasOps.Integrations.Webhooks.Contracts;
using AtlasOps.Integrations.Webhooks.Core;

[TestClass]
public sealed class WebhookSecurityServiceTests
{
    private static readonly DateTimeOffset Now = new(2026, 4, 5, 6, 7, 8, TimeSpan.Zero);
    private static readonly byte[] Key = "top-secret-key"u8.ToArray();

    [TestMethod]
    public void Sign_SamePayloadAndTimestamp_IsDeterministic()
    {
        WebhookSecurityService service = new();
        WebhookDelivery delivery = Delivery();

        WebhookSignature first = service.Sign(delivery, "key-1", Key, Now);
        WebhookSignature second = service.Sign(delivery, "key-1", Key, Now);

        Assert.AreEqual("hmac-sha256", first.Algorithm);
        Assert.AreEqual("key-1", first.KeyId);
        Assert.AreEqual(Now, first.Timestamp);
        Assert.AreEqual(first.Value, second.Value);
        Assert.AreEqual(64, first.Value.Length);
    }

    [TestMethod]
    public void Verify_ValidSignatureAtReplayWindowBoundary_Succeeds()
    {
        WebhookSecurityService service = new();
        WebhookDelivery delivery = Delivery();
        WebhookSignature signature = service.Sign(delivery, "key", Key, Now.AddMinutes(-5));

        WebhookVerificationResult result = service.Verify(
            delivery,
            signature,
            Key,
            Now,
            TimeSpan.FromMinutes(5),
            new HashSet<string>());

        Assert.IsTrue(result.Valid);
        Assert.IsFalse(result.ReplayDetected);
        Assert.AreEqual(string.Empty, result.Diagnostic);
    }

    [TestMethod]
    public void Verify_OutsidePastOrFutureWindowAndConsumedKey_DetectsReplay()
    {
        WebhookSecurityService service = new();
        WebhookDelivery delivery = Delivery();
        WebhookSignature past = service.Sign(delivery, "key", Key, Now.AddMinutes(-5).AddTicks(-1));
        WebhookSignature future = service.Sign(delivery, "key", Key, Now.AddMinutes(5).AddTicks(1));
        WebhookSignature current = service.Sign(delivery, "key", Key, Now);

        WebhookVerificationResult pastResult = service.Verify(
            delivery, past, Key, Now, TimeSpan.FromMinutes(5), new HashSet<string>());
        WebhookVerificationResult futureResult = service.Verify(
            delivery, future, Key, Now, TimeSpan.FromMinutes(5), new HashSet<string>());
        WebhookVerificationResult consumedResult = service.Verify(
            delivery, current, Key, Now, TimeSpan.FromMinutes(5), new HashSet<string>([delivery.IdempotencyKey]));

        Assert.IsTrue(pastResult.ReplayDetected);
        Assert.IsTrue(futureResult.ReplayDetected);
        Assert.AreEqual("Idempotency key has already been consumed.", consumedResult.Diagnostic);
    }

    [TestMethod]
    public void Verify_UnsupportedMalformedAndSameLengthWrongSignatures_AreRejected()
    {
        WebhookSecurityService service = new();
        WebhookDelivery delivery = Delivery();
        WebhookSignature valid = service.Sign(delivery, "key", Key, Now);
        string wrongSameLength = new('0', valid.Value.Length);

        WebhookVerificationResult unsupported = service.Verify(
            delivery, valid with { Algorithm = "sha1" }, Key, Now, TimeSpan.FromMinutes(1), new HashSet<string>());
        WebhookVerificationResult malformed = service.Verify(
            delivery, valid with { Value = "not-hex" }, Key, Now, TimeSpan.FromMinutes(1), new HashSet<string>());
        WebhookVerificationResult mismatch = service.Verify(
            delivery, valid with { Value = wrongSameLength }, Key, Now, TimeSpan.FromMinutes(1), new HashSet<string>());

        Assert.AreEqual("Signature algorithm is unsupported.", unsupported.Diagnostic);
        Assert.AreEqual("Signature encoding is invalid.", malformed.Diagnostic);
        Assert.AreEqual("Signature does not match.", mismatch.Diagnostic);
        Assert.IsFalse(mismatch.Valid);
        Assert.IsFalse(mismatch.ReplayDetected);
    }

    [TestMethod]
    public void PlanRetry_ExhaustionExponentialRetryAfterAndCap_AreApplied()
    {
        WebhookSecurityService service = new();

        WebhookRetryPlan exhausted = service.PlanRetry(3, 3, Now, null, "failure");
        WebhookRetryPlan exponential = service.PlanRetry(2, 3, Now, null, "failure");
        WebhookRetryPlan retryAfter = service.PlanRetry(0, 3, Now, 120, "failure");
        WebhookRetryPlan capped = service.PlanRetry(0, 3, Now, 4_000, "failure");

        Assert.IsFalse(exhausted.Retry);
        Assert.AreEqual(3, exhausted.NextAttempt);
        Assert.AreEqual(Now.AddSeconds(4), exponential.AvailableAt);
        Assert.AreEqual(3, exponential.NextAttempt);
        Assert.AreEqual(Now.AddSeconds(120), retryAfter.AvailableAt);
        Assert.AreEqual(Now.AddHours(1), capped.AvailableAt);
    }

    private static WebhookDelivery Delivery()
    {
        return new WebhookDelivery(
            Guid.Parse("11111111-2222-3333-4444-555555555555"),
            new Uri("https://example.invalid/hook"),
            "event",
            "application/json",
            "{}"u8.ToArray(),
            Now,
            "idempotency-1",
            new Dictionary<string, string>());
    }
}
