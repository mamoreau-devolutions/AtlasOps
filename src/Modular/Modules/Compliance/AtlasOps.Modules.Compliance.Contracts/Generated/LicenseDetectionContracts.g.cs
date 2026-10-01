namespace AtlasOps.Modules.Compliance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LicenseDetectionState { Draft, Active, Paused, Completed, Archived }
public sealed record LicenseDetectionRecord(Guid Id, string Name, string Owner, LicenseDetectionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LicenseDetectionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LicenseDetectionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LicenseDetectionQuery(string? SearchText, LicenseDetectionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LicenseDetectionPage(IReadOnlyList<LicenseDetectionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LicenseDetectionMutation(bool Succeeded, string Code, string Message, LicenseDetectionRecord? Record, LicenseDetectionEvent? Event);
public interface ILicenseDetectionRepository
{
    ValueTask<LicenseDetectionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LicenseDetectionPage> QueryAsync(LicenseDetectionQuery query, CancellationToken cancellationToken);
    ValueTask<LicenseDetectionMutation> SaveAsync(LicenseDetectionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILicenseDetectionEventSink { ValueTask PublishAsync(LicenseDetectionEvent domainEvent, CancellationToken cancellationToken); }