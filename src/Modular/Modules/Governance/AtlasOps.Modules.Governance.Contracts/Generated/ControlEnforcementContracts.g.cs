namespace AtlasOps.Modules.Governance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ControlEnforcementState { Draft, Active, Paused, Completed, Archived }
public sealed record ControlEnforcementRecord(Guid Id, string Name, string Owner, ControlEnforcementState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ControlEnforcementCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ControlEnforcementEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ControlEnforcementQuery(string? SearchText, ControlEnforcementState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ControlEnforcementPage(IReadOnlyList<ControlEnforcementRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ControlEnforcementMutation(bool Succeeded, string Code, string Message, ControlEnforcementRecord? Record, ControlEnforcementEvent? Event);
public interface IControlEnforcementRepository
{
    ValueTask<ControlEnforcementRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ControlEnforcementPage> QueryAsync(ControlEnforcementQuery query, CancellationToken cancellationToken);
    ValueTask<ControlEnforcementMutation> SaveAsync(ControlEnforcementRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IControlEnforcementEventSink { ValueTask PublishAsync(ControlEnforcementEvent domainEvent, CancellationToken cancellationToken); }