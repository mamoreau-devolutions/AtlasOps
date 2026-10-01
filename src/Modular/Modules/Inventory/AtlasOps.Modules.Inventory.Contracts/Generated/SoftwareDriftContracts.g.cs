namespace AtlasOps.Modules.Inventory.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum SoftwareDriftState { Draft, Active, Paused, Completed, Archived }
public sealed record SoftwareDriftRecord(Guid Id, string Name, string Owner, SoftwareDriftState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record SoftwareDriftCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record SoftwareDriftEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record SoftwareDriftQuery(string? SearchText, SoftwareDriftState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record SoftwareDriftPage(IReadOnlyList<SoftwareDriftRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record SoftwareDriftMutation(bool Succeeded, string Code, string Message, SoftwareDriftRecord? Record, SoftwareDriftEvent? Event);
public interface ISoftwareDriftRepository
{
    ValueTask<SoftwareDriftRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<SoftwareDriftPage> QueryAsync(SoftwareDriftQuery query, CancellationToken cancellationToken);
    ValueTask<SoftwareDriftMutation> SaveAsync(SoftwareDriftRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ISoftwareDriftEventSink { ValueTask PublishAsync(SoftwareDriftEvent domainEvent, CancellationToken cancellationToken); }