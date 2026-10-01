namespace AtlasOps.Modules.Compliance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LicenseRemediationState { Draft, Active, Paused, Completed, Archived }
public sealed record LicenseRemediationRecord(Guid Id, string Name, string Owner, LicenseRemediationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LicenseRemediationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LicenseRemediationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LicenseRemediationQuery(string? SearchText, LicenseRemediationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LicenseRemediationPage(IReadOnlyList<LicenseRemediationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LicenseRemediationMutation(bool Succeeded, string Code, string Message, LicenseRemediationRecord? Record, LicenseRemediationEvent? Event);
public interface ILicenseRemediationRepository
{
    ValueTask<LicenseRemediationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LicenseRemediationPage> QueryAsync(LicenseRemediationQuery query, CancellationToken cancellationToken);
    ValueTask<LicenseRemediationMutation> SaveAsync(LicenseRemediationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILicenseRemediationEventSink { ValueTask PublishAsync(LicenseRemediationEvent domainEvent, CancellationToken cancellationToken); }