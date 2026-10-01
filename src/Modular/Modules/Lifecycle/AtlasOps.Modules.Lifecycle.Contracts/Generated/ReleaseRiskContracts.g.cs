namespace AtlasOps.Modules.Lifecycle.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ReleaseRiskState { Draft, Active, Paused, Completed, Archived }
public sealed record ReleaseRiskRecord(Guid Id, string Name, string Owner, ReleaseRiskState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ReleaseRiskCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ReleaseRiskEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ReleaseRiskQuery(string? SearchText, ReleaseRiskState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ReleaseRiskPage(IReadOnlyList<ReleaseRiskRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ReleaseRiskMutation(bool Succeeded, string Code, string Message, ReleaseRiskRecord? Record, ReleaseRiskEvent? Event);
public interface IReleaseRiskRepository
{
    ValueTask<ReleaseRiskRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ReleaseRiskPage> QueryAsync(ReleaseRiskQuery query, CancellationToken cancellationToken);
    ValueTask<ReleaseRiskMutation> SaveAsync(ReleaseRiskRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IReleaseRiskEventSink { ValueTask PublishAsync(ReleaseRiskEvent domainEvent, CancellationToken cancellationToken); }