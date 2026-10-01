namespace AtlasOps.Modules.Inventory.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum DependencyDriftState { Draft, Active, Paused, Completed, Archived }
public sealed record DependencyDriftRecord(Guid Id, string Name, string Owner, DependencyDriftState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record DependencyDriftCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record DependencyDriftEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record DependencyDriftQuery(string? SearchText, DependencyDriftState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record DependencyDriftPage(IReadOnlyList<DependencyDriftRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record DependencyDriftMutation(bool Succeeded, string Code, string Message, DependencyDriftRecord? Record, DependencyDriftEvent? Event);
public interface IDependencyDriftRepository
{
    ValueTask<DependencyDriftRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<DependencyDriftPage> QueryAsync(DependencyDriftQuery query, CancellationToken cancellationToken);
    ValueTask<DependencyDriftMutation> SaveAsync(DependencyDriftRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IDependencyDriftEventSink { ValueTask PublishAsync(DependencyDriftEvent domainEvent, CancellationToken cancellationToken); }