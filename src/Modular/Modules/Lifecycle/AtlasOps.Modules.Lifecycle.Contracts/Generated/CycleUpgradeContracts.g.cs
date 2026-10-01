namespace AtlasOps.Modules.Lifecycle.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum CycleUpgradeState { Draft, Active, Paused, Completed, Archived }
public sealed record CycleUpgradeRecord(Guid Id, string Name, string Owner, CycleUpgradeState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record CycleUpgradeCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CycleUpgradeEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record CycleUpgradeQuery(string? SearchText, CycleUpgradeState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record CycleUpgradePage(IReadOnlyList<CycleUpgradeRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record CycleUpgradeMutation(bool Succeeded, string Code, string Message, CycleUpgradeRecord? Record, CycleUpgradeEvent? Event);
public interface ICycleUpgradeRepository
{
    ValueTask<CycleUpgradeRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<CycleUpgradePage> QueryAsync(CycleUpgradeQuery query, CancellationToken cancellationToken);
    ValueTask<CycleUpgradeMutation> SaveAsync(CycleUpgradeRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ICycleUpgradeEventSink { ValueTask PublishAsync(CycleUpgradeEvent domainEvent, CancellationToken cancellationToken); }