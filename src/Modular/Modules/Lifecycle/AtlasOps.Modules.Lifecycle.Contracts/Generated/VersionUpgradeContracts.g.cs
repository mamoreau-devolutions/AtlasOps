namespace AtlasOps.Modules.Lifecycle.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum VersionUpgradeState { Draft, Active, Paused, Completed, Archived }
public sealed record VersionUpgradeRecord(Guid Id, string Name, string Owner, VersionUpgradeState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record VersionUpgradeCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record VersionUpgradeEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record VersionUpgradeQuery(string? SearchText, VersionUpgradeState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record VersionUpgradePage(IReadOnlyList<VersionUpgradeRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record VersionUpgradeMutation(bool Succeeded, string Code, string Message, VersionUpgradeRecord? Record, VersionUpgradeEvent? Event);
public interface IVersionUpgradeRepository
{
    ValueTask<VersionUpgradeRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<VersionUpgradePage> QueryAsync(VersionUpgradeQuery query, CancellationToken cancellationToken);
    ValueTask<VersionUpgradeMutation> SaveAsync(VersionUpgradeRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IVersionUpgradeEventSink { ValueTask PublishAsync(VersionUpgradeEvent domainEvent, CancellationToken cancellationToken); }