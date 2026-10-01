namespace AtlasOps.Modules.Geography.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum DivisionOverrideState { Draft, Active, Paused, Completed, Archived }
public sealed record DivisionOverrideRecord(Guid Id, string Name, string Owner, DivisionOverrideState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record DivisionOverrideCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record DivisionOverrideEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record DivisionOverrideQuery(string? SearchText, DivisionOverrideState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record DivisionOverridePage(IReadOnlyList<DivisionOverrideRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record DivisionOverrideMutation(bool Succeeded, string Code, string Message, DivisionOverrideRecord? Record, DivisionOverrideEvent? Event);
public interface IDivisionOverrideRepository
{
    ValueTask<DivisionOverrideRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<DivisionOverridePage> QueryAsync(DivisionOverrideQuery query, CancellationToken cancellationToken);
    ValueTask<DivisionOverrideMutation> SaveAsync(DivisionOverrideRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IDivisionOverrideEventSink { ValueTask PublishAsync(DivisionOverrideEvent domainEvent, CancellationToken cancellationToken); }