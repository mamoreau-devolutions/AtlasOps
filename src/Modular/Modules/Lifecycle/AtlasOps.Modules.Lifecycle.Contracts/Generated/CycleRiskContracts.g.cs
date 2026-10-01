namespace AtlasOps.Modules.Lifecycle.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum CycleRiskState { Draft, Active, Paused, Completed, Archived }
public sealed record CycleRiskRecord(Guid Id, string Name, string Owner, CycleRiskState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record CycleRiskCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CycleRiskEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record CycleRiskQuery(string? SearchText, CycleRiskState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record CycleRiskPage(IReadOnlyList<CycleRiskRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record CycleRiskMutation(bool Succeeded, string Code, string Message, CycleRiskRecord? Record, CycleRiskEvent? Event);
public interface ICycleRiskRepository
{
    ValueTask<CycleRiskRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<CycleRiskPage> QueryAsync(CycleRiskQuery query, CancellationToken cancellationToken);
    ValueTask<CycleRiskMutation> SaveAsync(CycleRiskRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ICycleRiskEventSink { ValueTask PublishAsync(CycleRiskEvent domainEvent, CancellationToken cancellationToken); }