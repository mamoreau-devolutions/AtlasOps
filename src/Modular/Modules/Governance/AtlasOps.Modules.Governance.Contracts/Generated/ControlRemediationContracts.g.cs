namespace AtlasOps.Modules.Governance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ControlRemediationState { Draft, Active, Paused, Completed, Archived }
public sealed record ControlRemediationRecord(Guid Id, string Name, string Owner, ControlRemediationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ControlRemediationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ControlRemediationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ControlRemediationQuery(string? SearchText, ControlRemediationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ControlRemediationPage(IReadOnlyList<ControlRemediationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ControlRemediationMutation(bool Succeeded, string Code, string Message, ControlRemediationRecord? Record, ControlRemediationEvent? Event);
public interface IControlRemediationRepository
{
    ValueTask<ControlRemediationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ControlRemediationPage> QueryAsync(ControlRemediationQuery query, CancellationToken cancellationToken);
    ValueTask<ControlRemediationMutation> SaveAsync(ControlRemediationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IControlRemediationEventSink { ValueTask PublishAsync(ControlRemediationEvent domainEvent, CancellationToken cancellationToken); }