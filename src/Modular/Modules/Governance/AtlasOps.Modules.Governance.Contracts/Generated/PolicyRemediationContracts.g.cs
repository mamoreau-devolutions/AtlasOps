namespace AtlasOps.Modules.Governance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum PolicyRemediationState { Draft, Active, Paused, Completed, Archived }
public sealed record PolicyRemediationRecord(Guid Id, string Name, string Owner, PolicyRemediationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record PolicyRemediationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record PolicyRemediationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record PolicyRemediationQuery(string? SearchText, PolicyRemediationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record PolicyRemediationPage(IReadOnlyList<PolicyRemediationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record PolicyRemediationMutation(bool Succeeded, string Code, string Message, PolicyRemediationRecord? Record, PolicyRemediationEvent? Event);
public interface IPolicyRemediationRepository
{
    ValueTask<PolicyRemediationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<PolicyRemediationPage> QueryAsync(PolicyRemediationQuery query, CancellationToken cancellationToken);
    ValueTask<PolicyRemediationMutation> SaveAsync(PolicyRemediationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IPolicyRemediationEventSink { ValueTask PublishAsync(PolicyRemediationEvent domainEvent, CancellationToken cancellationToken); }