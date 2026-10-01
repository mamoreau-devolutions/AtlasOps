namespace AtlasOps.Modules.Compliance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum FindingRemediationState { Draft, Active, Paused, Completed, Archived }
public sealed record FindingRemediationRecord(Guid Id, string Name, string Owner, FindingRemediationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record FindingRemediationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record FindingRemediationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record FindingRemediationQuery(string? SearchText, FindingRemediationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record FindingRemediationPage(IReadOnlyList<FindingRemediationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record FindingRemediationMutation(bool Succeeded, string Code, string Message, FindingRemediationRecord? Record, FindingRemediationEvent? Event);
public interface IFindingRemediationRepository
{
    ValueTask<FindingRemediationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<FindingRemediationPage> QueryAsync(FindingRemediationQuery query, CancellationToken cancellationToken);
    ValueTask<FindingRemediationMutation> SaveAsync(FindingRemediationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IFindingRemediationEventSink { ValueTask PublishAsync(FindingRemediationEvent domainEvent, CancellationToken cancellationToken); }