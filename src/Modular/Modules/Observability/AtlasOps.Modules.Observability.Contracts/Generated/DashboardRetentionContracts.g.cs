namespace AtlasOps.Modules.Observability.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum DashboardRetentionState { Draft, Active, Paused, Completed, Archived }
public sealed record DashboardRetentionRecord(Guid Id, string Name, string Owner, DashboardRetentionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record DashboardRetentionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record DashboardRetentionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record DashboardRetentionQuery(string? SearchText, DashboardRetentionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record DashboardRetentionPage(IReadOnlyList<DashboardRetentionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record DashboardRetentionMutation(bool Succeeded, string Code, string Message, DashboardRetentionRecord? Record, DashboardRetentionEvent? Event);
public interface IDashboardRetentionRepository
{
    ValueTask<DashboardRetentionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<DashboardRetentionPage> QueryAsync(DashboardRetentionQuery query, CancellationToken cancellationToken);
    ValueTask<DashboardRetentionMutation> SaveAsync(DashboardRetentionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IDashboardRetentionEventSink { ValueTask PublishAsync(DashboardRetentionEvent domainEvent, CancellationToken cancellationToken); }