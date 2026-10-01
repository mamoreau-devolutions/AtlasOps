namespace AtlasOps.Modules.Workspaces.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum MilestoneForecastingState { Draft, Active, Paused, Completed, Archived }
public sealed record MilestoneForecastingRecord(Guid Id, string Name, string Owner, MilestoneForecastingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record MilestoneForecastingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record MilestoneForecastingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record MilestoneForecastingQuery(string? SearchText, MilestoneForecastingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record MilestoneForecastingPage(IReadOnlyList<MilestoneForecastingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record MilestoneForecastingMutation(bool Succeeded, string Code, string Message, MilestoneForecastingRecord? Record, MilestoneForecastingEvent? Event);
public interface IMilestoneForecastingRepository
{
    ValueTask<MilestoneForecastingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<MilestoneForecastingPage> QueryAsync(MilestoneForecastingQuery query, CancellationToken cancellationToken);
    ValueTask<MilestoneForecastingMutation> SaveAsync(MilestoneForecastingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IMilestoneForecastingEventSink { ValueTask PublishAsync(MilestoneForecastingEvent domainEvent, CancellationToken cancellationToken); }