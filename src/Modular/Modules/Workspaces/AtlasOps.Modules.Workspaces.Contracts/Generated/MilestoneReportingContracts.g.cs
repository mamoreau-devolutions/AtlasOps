namespace AtlasOps.Modules.Workspaces.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum MilestoneReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record MilestoneReportingRecord(Guid Id, string Name, string Owner, MilestoneReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record MilestoneReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record MilestoneReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record MilestoneReportingQuery(string? SearchText, MilestoneReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record MilestoneReportingPage(IReadOnlyList<MilestoneReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record MilestoneReportingMutation(bool Succeeded, string Code, string Message, MilestoneReportingRecord? Record, MilestoneReportingEvent? Event);
public interface IMilestoneReportingRepository
{
    ValueTask<MilestoneReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<MilestoneReportingPage> QueryAsync(MilestoneReportingQuery query, CancellationToken cancellationToken);
    ValueTask<MilestoneReportingMutation> SaveAsync(MilestoneReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IMilestoneReportingEventSink { ValueTask PublishAsync(MilestoneReportingEvent domainEvent, CancellationToken cancellationToken); }