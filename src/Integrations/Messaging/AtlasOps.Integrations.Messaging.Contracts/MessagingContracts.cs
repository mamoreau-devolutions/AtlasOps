namespace AtlasOps.Integrations.Messaging.Contracts;

public sealed record MessageEnvelope(
    string BrokerId,
    string Destination,
    string MessageId,
    string? PartitionKey,
    string ContentType,
    byte[] Payload,
    DateTimeOffset EnqueuedAt,
    IReadOnlyDictionary<string, string> Headers);

public enum DeliveryDisposition
{
    Acknowledge,
    Retry,
    DeadLetter,
    Reject,
}

public sealed record DeliveryDecision(
    DeliveryDisposition Disposition,
    DateTimeOffset? RetryAt,
    string Diagnostic);

public sealed record DeadLetterRecord(
    MessageEnvelope Message,
    string Reason,
    int DeliveryAttempt,
    DateTimeOffset FailedAt);
