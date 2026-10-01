namespace AtlasOps.Integrations.Webhooks.Contracts;

public sealed record WebhookDelivery(
    Guid Id,
    Uri Endpoint,
    string EventName,
    string ContentType,
    byte[] Payload,
    DateTimeOffset CreatedAt,
    string IdempotencyKey,
    IReadOnlyDictionary<string, string> Headers);

public sealed record WebhookSignature(
    string Algorithm,
    string KeyId,
    DateTimeOffset Timestamp,
    string Value);

public sealed record WebhookVerificationResult(
    bool Valid,
    bool ReplayDetected,
    string Diagnostic);

public sealed record WebhookRetryPlan(
    bool Retry,
    int NextAttempt,
    DateTimeOffset AvailableAt,
    string Diagnostic);
