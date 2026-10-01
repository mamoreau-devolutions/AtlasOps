namespace AtlasOps.Modules.CloudEconomics.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum CommitmentForecastingState { Draft, Active, Paused, Completed, Archived }
public sealed record CommitmentForecastingRecord(Guid Id, string Name, string Owner, CommitmentForecastingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record CommitmentForecastingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CommitmentForecastingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record CommitmentForecastingQuery(string? SearchText, CommitmentForecastingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record CommitmentForecastingPage(IReadOnlyList<CommitmentForecastingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record CommitmentForecastingMutation(bool Succeeded, string Code, string Message, CommitmentForecastingRecord? Record, CommitmentForecastingEvent? Event);
public interface ICommitmentForecastingRepository
{
    ValueTask<CommitmentForecastingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<CommitmentForecastingPage> QueryAsync(CommitmentForecastingQuery query, CancellationToken cancellationToken);
    ValueTask<CommitmentForecastingMutation> SaveAsync(CommitmentForecastingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ICommitmentForecastingEventSink { ValueTask PublishAsync(CommitmentForecastingEvent domainEvent, CancellationToken cancellationToken); }