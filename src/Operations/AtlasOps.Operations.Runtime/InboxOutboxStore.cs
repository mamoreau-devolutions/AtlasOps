namespace AtlasOps.Operations.Runtime;

using System.Security.Cryptography;

using AtlasOps.Operations.Contracts;

public sealed class InboxOutboxStore
{
    private readonly object gate = new();
    private readonly Dictionary<string, InboxMessage> inbox = new(StringComparer.Ordinal);
    private readonly Dictionary<Guid, OutboxMessage> outbox = [];

    public InboxAcceptanceResult Accept(InboxMessage message)
    {
        if (string.IsNullOrWhiteSpace(message.ProviderId) ||
            string.IsNullOrWhiteSpace(message.MessageId))
        {
            return new InboxAcceptanceResult(false, false, "Provider and message IDs are required.");
        }

        string computedHash = Convert.ToHexString(SHA256.HashData(message.Payload));
        if (!string.Equals(computedHash, message.ContentHash, StringComparison.OrdinalIgnoreCase))
        {
            return new InboxAcceptanceResult(false, false, "The payload hash does not match.");
        }

        string key = CreateInboxKey(message.ProviderId, message.MessageId);
        lock (this.gate)
        {
            if (this.inbox.TryGetValue(key, out InboxMessage? existing))
            {
                bool sameContent = string.Equals(
                    existing.ContentHash,
                    message.ContentHash,
                    StringComparison.OrdinalIgnoreCase);
                return sameContent
                    ? new InboxAcceptanceResult(true, true, string.Empty)
                    : new InboxAcceptanceResult(false, true, "The duplicate message has different content.");
            }

            this.inbox.Add(key, message);
            return new InboxAcceptanceResult(true, false, string.Empty);
        }
    }

    public bool AddOutbox(OutboxMessage message)
    {
        if (message.Id == Guid.Empty ||
            string.IsNullOrWhiteSpace(message.Destination) ||
            message.Payload.Length == 0)
        {
            return false;
        }

        lock (this.gate)
        {
            return this.outbox.TryAdd(message.Id, message);
        }
    }

    public IReadOnlyList<OutboxMessage> GetPending(int maximumCount)
    {
        if (maximumCount < 1)
        {
            return [];
        }

        lock (this.gate)
        {
            return this.outbox.Values
                .Where(static message => message.PublishedAt is null)
                .OrderBy(static message => message.CreatedAt)
                .ThenBy(static message => message.Id)
                .Take(maximumCount)
                .ToArray();
        }
    }

    public bool MarkPublished(Guid messageId, DateTimeOffset publishedAt)
    {
        lock (this.gate)
        {
            if (!this.outbox.TryGetValue(messageId, out OutboxMessage? message) ||
                message.PublishedAt is not null)
            {
                return false;
            }

            this.outbox[messageId] = message with
            {
                PublishedAt = publishedAt,
                LastDiagnostic = null,
            };
            return true;
        }
    }

    public bool MarkFailed(Guid messageId, string diagnostic)
    {
        lock (this.gate)
        {
            if (!this.outbox.TryGetValue(messageId, out OutboxMessage? message) ||
                message.PublishedAt is not null)
            {
                return false;
            }

            this.outbox[messageId] = message with
            {
                Attempt = message.Attempt + 1,
                LastDiagnostic = diagnostic,
            };
            return true;
        }
    }

    private static string CreateInboxKey(string providerId, string messageId)
    {
        return $"{providerId.Trim().ToUpperInvariant()}:{messageId.Trim()}";
    }
}
