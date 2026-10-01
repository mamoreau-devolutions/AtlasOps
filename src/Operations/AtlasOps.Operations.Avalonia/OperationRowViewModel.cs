namespace AtlasOps.Operations.Avalonia;

using AtlasOps.Operations.Contracts;

public sealed record OperationRowViewModel(
    string Id,
    string Title,
    string Category,
    string Status,
    string Detail,
    DateTimeOffset UpdatedAt,
    string Severity)
{
    public static OperationRowViewModel FromJob(DurableJob job)
    {
        string severity = job.Status switch
        {
            DurableJobStatus.Failed or DurableJobStatus.DeadLettered => "Error",
            DurableJobStatus.WaitingForRetry => "Warning",
            DurableJobStatus.Succeeded => "Success",
            _ => "Information",
        };
        return new OperationRowViewModel(
            job.Id.ToString("D"),
            job.Envelope.Operation,
            job.Envelope.ProviderId,
            job.Status.ToString(),
            job.LastDiagnostic ?? "No diagnostic was reported.",
            job.UpdatedAt,
            severity);
    }
}
