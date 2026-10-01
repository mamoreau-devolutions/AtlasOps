namespace AtlasOps.Integrations.Messaging.Core;

using AtlasOps.Integrations.Messaging.Contracts;

public sealed class MessageDeliveryService
{
    private readonly object gate = new();
    private readonly HashSet<string> acceptedMessages = new(StringComparer.Ordinal);
    private readonly List<DeadLetterRecord> deadLetters = [];

    public DeliveryDecision Decide(
        MessageEnvelope message,
        int attempt,
        int maximumAttempts,
        bool transientFailure,
        string diagnostic,
        DateTimeOffset now)
    {
        if (string.IsNullOrWhiteSpace(message.MessageId) ||
            string.IsNullOrWhiteSpace(message.Destination) ||
            message.Payload.Length == 0)
        {
            return new DeliveryDecision(DeliveryDisposition.Reject, null, "Message is invalid.");
        }

        if (!transientFailure)
        {
            return new DeliveryDecision(DeliveryDisposition.DeadLetter, null, diagnostic);
        }

        if (attempt >= maximumAttempts)
        {
            return new DeliveryDecision(
                DeliveryDisposition.DeadLetter,
                null,
                "Maximum delivery attempts were exhausted.");
        }

        int exponent = Math.Min(10, Math.Max(0, attempt - 1));
        TimeSpan delay = TimeSpan.FromSeconds(Math.Pow(2d, exponent));
        return new DeliveryDecision(DeliveryDisposition.Retry, now.Add(delay), diagnostic);
    }

    public bool TryAcknowledge(MessageEnvelope message)
    {
        string key = CreateKey(message);
        lock (this.gate)
        {
            return this.acceptedMessages.Add(key);
        }
    }

    public DeadLetterRecord DeadLetter(
        MessageEnvelope message,
        string reason,
        int attempt,
        DateTimeOffset failedAt)
    {
        DeadLetterRecord record = new(message, reason, attempt, failedAt);
        lock (this.gate)
        {
            this.deadLetters.Add(record);
        }

        return record;
    }

    public IReadOnlyList<DeadLetterRecord> GetDeadLetters()
    {
        lock (this.gate)
        {
            return this.deadLetters
                .OrderByDescending(static record => record.FailedAt)
                .ToArray();
        }
    }

    private static string CreateKey(MessageEnvelope message)
    {
        return $"{message.BrokerId.Trim().ToUpperInvariant()}:{message.MessageId.Trim()}";
    }
}
