namespace AtlasOps.Modules.Compliance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ComponentRemediationState { Draft, Active, Paused, Completed, Archived }
public sealed record ComponentRemediationRecord(Guid Id, string Name, string Owner, ComponentRemediationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ComponentRemediationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ComponentRemediationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ComponentRemediationQuery(string? SearchText, ComponentRemediationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ComponentRemediationPage(IReadOnlyList<ComponentRemediationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ComponentRemediationMutation(bool Succeeded, string Code, string Message, ComponentRemediationRecord? Record, ComponentRemediationEvent? Event);
public interface IComponentRemediationRepository
{
    ValueTask<ComponentRemediationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ComponentRemediationPage> QueryAsync(ComponentRemediationQuery query, CancellationToken cancellationToken);
    ValueTask<ComponentRemediationMutation> SaveAsync(ComponentRemediationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IComponentRemediationEventSink { ValueTask PublishAsync(ComponentRemediationEvent domainEvent, CancellationToken cancellationToken); }