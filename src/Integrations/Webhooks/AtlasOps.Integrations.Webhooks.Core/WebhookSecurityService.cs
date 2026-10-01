namespace AtlasOps.Integrations.Webhooks.Core;

using System.Globalization;
using System.Security.Cryptography;
using System.Text;

using AtlasOps.Integrations.Webhooks.Contracts;

public sealed class WebhookSecurityService
{
    public WebhookSignature Sign(
        WebhookDelivery delivery,
        string keyId,
        byte[] key,
        DateTimeOffset timestamp)
    {
        byte[] canonical = CreateCanonicalPayload(delivery, timestamp);
        byte[] signature = HMACSHA256.HashData(key, canonical);
        return new WebhookSignature(
            "hmac-sha256",
            keyId,
            timestamp,
            Convert.ToHexString(signature));
    }

    public WebhookVerificationResult Verify(
        WebhookDelivery delivery,
        WebhookSignature signature,
        byte[] key,
        DateTimeOffset now,
        TimeSpan replayWindow,
        IReadOnlySet<string> consumedIdempotencyKeys)
    {
        if (!string.Equals(signature.Algorithm, "hmac-sha256", StringComparison.OrdinalIgnoreCase))
        {
            return new WebhookVerificationResult(false, false, "Signature algorithm is unsupported.");
        }

        TimeSpan age = now - signature.Timestamp;
        if (age.Duration() > replayWindow)
        {
            return new WebhookVerificationResult(false, true, "Delivery timestamp is outside the replay window.");
        }

        if (consumedIdempotencyKeys.Contains(delivery.IdempotencyKey))
        {
            return new WebhookVerificationResult(false, true, "Idempotency key has already been consumed.");
        }

        byte[] canonical = CreateCanonicalPayload(delivery, signature.Timestamp);
        byte[] expected = HMACSHA256.HashData(key, canonical);
        byte[] supplied;
        try
        {
            supplied = Convert.FromHexString(signature.Value);
        }
        catch (FormatException)
        {
            return new WebhookVerificationResult(false, false, "Signature encoding is invalid.");
        }

        bool valid = expected.Length == supplied.Length &&
                     CryptographicOperations.FixedTimeEquals(expected, supplied);
        return valid
            ? new WebhookVerificationResult(true, false, string.Empty)
            : new WebhookVerificationResult(false, false, "Signature does not match.");
    }

    public WebhookRetryPlan PlanRetry(
        int currentAttempt,
        int maximumAttempts,
        DateTimeOffset now,
        int? retryAfterSeconds,
        string diagnostic)
    {
        if (currentAttempt >= maximumAttempts)
        {
            return new WebhookRetryPlan(false, currentAttempt, now, "Maximum attempts were exhausted.");
        }

        int nextAttempt = currentAttempt + 1;
        int delaySeconds = retryAfterSeconds is > 0
            ? Math.Min(retryAfterSeconds.Value, 3_600)
            : Math.Min(3_600, (int)Math.Pow(2d, Math.Min(10, currentAttempt)));
        return new WebhookRetryPlan(true, nextAttempt, now.AddSeconds(delaySeconds), diagnostic);
    }

    private static byte[] CreateCanonicalPayload(
        WebhookDelivery delivery,
        DateTimeOffset timestamp)
    {
        string prefix = string.Join(
            "\n",
            delivery.Id.ToString("D"),
            delivery.EventName,
            delivery.ContentType,
            delivery.IdempotencyKey,
            timestamp.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture),
            Convert.ToHexString(SHA256.HashData(delivery.Payload)));
        return Encoding.UTF8.GetBytes(prefix);
    }
}
