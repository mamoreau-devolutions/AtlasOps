namespace AtlasOps.Operations.Tests;

using AtlasOps.Operations.Contracts;

internal static class OperationTestData
{
    public static readonly DateTimeOffset Now = new(2026, 1, 2, 3, 4, 5, TimeSpan.Zero);

    public static OperationEnvelope Envelope(string operation = "sync")
    {
        return new OperationEnvelope(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            "provider",
            operation,
            Now,
            "credential",
            new Dictionary<string, string>());
    }

    public static DurableJob Job(
        DurableJobStatus status = DurableJobStatus.Pending,
        int attempt = 0,
        int maximumAttempts = 3,
        long revision = 0,
        DateTimeOffset? availableAt = null,
        DateTimeOffset? createdAt = null,
        Guid? id = null,
        string operation = "sync")
    {
        return new DurableJob(
            id ?? Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            Envelope(operation),
            status,
            attempt,
            maximumAttempts,
            availableAt ?? Now,
            createdAt ?? Now,
            Now,
            revision,
            null);
    }

    public static OperationExecutionResult Result(
        bool succeeded,
        OperationFailureKind kind = OperationFailureKind.None,
        string diagnostic = "")
    {
        return new OperationExecutionResult(
            Envelope().Id,
            succeeded,
            kind,
            diagnostic,
            null,
            OperationContract.EmptyDetails);
    }
}
