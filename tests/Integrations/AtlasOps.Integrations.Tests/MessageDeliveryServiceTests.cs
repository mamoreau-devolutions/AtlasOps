namespace AtlasOps.Integrations.Tests;

using AtlasOps.Integrations.Messaging.Contracts;
using AtlasOps.Integrations.Messaging.Core;

[TestClass]
public sealed class MessageDeliveryServiceTests
{
    private static readonly DateTimeOffset Now = new(2026, 3, 4, 5, 6, 7, TimeSpan.Zero);

    [TestMethod]
    public void Decide_InvalidPermanentAndExhaustedMessages_AreRejectedOrDeadLettered()
    {
        MessageDeliveryService service = new();

        DeliveryDecision invalid = service.Decide(Message("") with { Payload = [] }, 1, 3, true, "invalid", Now);
        DeliveryDecision permanent = service.Decide(Message("1"), 1, 3, false, "permanent", Now);
        DeliveryDecision exhausted = service.Decide(Message("2"), 3, 3, true, "transient", Now);

        Assert.AreEqual(DeliveryDisposition.Reject, invalid.Disposition);
        Assert.AreEqual("Message is invalid.", invalid.Diagnostic);
        Assert.AreEqual(DeliveryDisposition.DeadLetter, permanent.Disposition);
        Assert.AreEqual("permanent", permanent.Diagnostic);
        Assert.AreEqual(DeliveryDisposition.DeadLetter, exhausted.Disposition);
        Assert.AreEqual("Maximum delivery attempts were exhausted.", exhausted.Diagnostic);
    }

    [TestMethod]
    public void Decide_TransientFailureUsesExponentialBackoffAndCapsExponent()
    {
        MessageDeliveryService service = new();

        DeliveryDecision first = service.Decide(Message("1"), 1, 20, true, "retry", Now);
        DeliveryDecision capped = service.Decide(Message("2"), 50, 100, true, "retry", Now);

        Assert.AreEqual(DeliveryDisposition.Retry, first.Disposition);
        Assert.AreEqual(Now.AddSeconds(1), first.RetryAt);
        Assert.AreEqual(Now.AddSeconds(1_024), capped.RetryAt);
    }

    [TestMethod]
    public void TryAcknowledge_NormalizesBrokerAndMessageIdAndRejectsDuplicate()
    {
        MessageDeliveryService service = new();
        MessageEnvelope message = Message(" id ") with { BrokerId = " broker " };

        bool first = service.TryAcknowledge(message);
        bool duplicate = service.TryAcknowledge(message with { BrokerId = "BROKER", MessageId = "id" });

        Assert.IsTrue(first);
        Assert.IsFalse(duplicate);
    }

    [TestMethod]
    public void DeadLetter_GetDeadLettersReturnsNewestFirstWithRecordFidelity()
    {
        MessageDeliveryService service = new();
        DeadLetterRecord older = service.DeadLetter(Message("1"), "older", 2, Now);
        DeadLetterRecord newer = service.DeadLetter(Message("2"), "newer", 3, Now.AddMinutes(1));

        IReadOnlyList<DeadLetterRecord> result = service.GetDeadLetters();

        CollectionAssert.AreEqual(new[] { newer, older }, result.ToArray());
        Assert.AreEqual(3, result[0].DeliveryAttempt);
        Assert.AreEqual("newer", result[0].Reason);
    }

    private static MessageEnvelope Message(string id)
    {
        return new MessageEnvelope(
            "broker",
            "queue",
            id,
            null,
            "text/plain",
            [1],
            Now,
            new Dictionary<string, string>());
    }
}
