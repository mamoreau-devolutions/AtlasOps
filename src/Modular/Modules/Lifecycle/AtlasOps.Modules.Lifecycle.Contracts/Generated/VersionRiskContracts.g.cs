namespace AtlasOps.Modules.Lifecycle.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum VersionRiskState { Draft, Active, Paused, Completed, Archived }
public sealed record VersionRiskRecord(Guid Id, string Name, string Owner, VersionRiskState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record VersionRiskCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record VersionRiskEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record VersionRiskQuery(string? SearchText, VersionRiskState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record VersionRiskPage(IReadOnlyList<VersionRiskRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record VersionRiskMutation(bool Succeeded, string Code, string Message, VersionRiskRecord? Record, VersionRiskEvent? Event);
public interface IVersionRiskRepository
{
    ValueTask<VersionRiskRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<VersionRiskPage> QueryAsync(VersionRiskQuery query, CancellationToken cancellationToken);
    ValueTask<VersionRiskMutation> SaveAsync(VersionRiskRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IVersionRiskEventSink { ValueTask PublishAsync(VersionRiskEvent domainEvent, CancellationToken cancellationToken); }