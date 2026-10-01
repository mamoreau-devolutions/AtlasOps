namespace AtlasOps.Modules.Compliance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum FindingReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record FindingReportingRecord(Guid Id, string Name, string Owner, FindingReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record FindingReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record FindingReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record FindingReportingQuery(string? SearchText, FindingReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record FindingReportingPage(IReadOnlyList<FindingReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record FindingReportingMutation(bool Succeeded, string Code, string Message, FindingReportingRecord? Record, FindingReportingEvent? Event);
public interface IFindingReportingRepository
{
    ValueTask<FindingReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<FindingReportingPage> QueryAsync(FindingReportingQuery query, CancellationToken cancellationToken);
    ValueTask<FindingReportingMutation> SaveAsync(FindingReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IFindingReportingEventSink { ValueTask PublishAsync(FindingReportingEvent domainEvent, CancellationToken cancellationToken); }