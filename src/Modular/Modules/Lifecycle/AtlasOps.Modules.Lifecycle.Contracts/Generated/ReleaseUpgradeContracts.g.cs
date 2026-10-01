namespace AtlasOps.Modules.Lifecycle.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ReleaseUpgradeState { Draft, Active, Paused, Completed, Archived }
public sealed record ReleaseUpgradeRecord(Guid Id, string Name, string Owner, ReleaseUpgradeState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ReleaseUpgradeCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ReleaseUpgradeEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ReleaseUpgradeQuery(string? SearchText, ReleaseUpgradeState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ReleaseUpgradePage(IReadOnlyList<ReleaseUpgradeRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ReleaseUpgradeMutation(bool Succeeded, string Code, string Message, ReleaseUpgradeRecord? Record, ReleaseUpgradeEvent? Event);
public interface IReleaseUpgradeRepository
{
    ValueTask<ReleaseUpgradeRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ReleaseUpgradePage> QueryAsync(ReleaseUpgradeQuery query, CancellationToken cancellationToken);
    ValueTask<ReleaseUpgradeMutation> SaveAsync(ReleaseUpgradeRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IReleaseUpgradeEventSink { ValueTask PublishAsync(ReleaseUpgradeEvent domainEvent, CancellationToken cancellationToken); }