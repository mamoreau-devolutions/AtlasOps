namespace AtlasOps.Modules.Inventory.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum DependencyReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record DependencyReportingRecord(Guid Id, string Name, string Owner, DependencyReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record DependencyReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record DependencyReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record DependencyReportingQuery(string? SearchText, DependencyReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record DependencyReportingPage(IReadOnlyList<DependencyReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record DependencyReportingMutation(bool Succeeded, string Code, string Message, DependencyReportingRecord? Record, DependencyReportingEvent? Event);
public interface IDependencyReportingRepository
{
    ValueTask<DependencyReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<DependencyReportingPage> QueryAsync(DependencyReportingQuery query, CancellationToken cancellationToken);
    ValueTask<DependencyReportingMutation> SaveAsync(DependencyReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IDependencyReportingEventSink { ValueTask PublishAsync(DependencyReportingEvent domainEvent, CancellationToken cancellationToken); }