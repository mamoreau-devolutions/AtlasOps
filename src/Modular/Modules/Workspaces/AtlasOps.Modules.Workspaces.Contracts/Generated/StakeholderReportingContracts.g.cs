namespace AtlasOps.Modules.Workspaces.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum StakeholderReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record StakeholderReportingRecord(Guid Id, string Name, string Owner, StakeholderReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record StakeholderReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record StakeholderReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record StakeholderReportingQuery(string? SearchText, StakeholderReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record StakeholderReportingPage(IReadOnlyList<StakeholderReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record StakeholderReportingMutation(bool Succeeded, string Code, string Message, StakeholderReportingRecord? Record, StakeholderReportingEvent? Event);
public interface IStakeholderReportingRepository
{
    ValueTask<StakeholderReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<StakeholderReportingPage> QueryAsync(StakeholderReportingQuery query, CancellationToken cancellationToken);
    ValueTask<StakeholderReportingMutation> SaveAsync(StakeholderReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IStakeholderReportingEventSink { ValueTask PublishAsync(StakeholderReportingEvent domainEvent, CancellationToken cancellationToken); }