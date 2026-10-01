namespace AtlasOps.Modules.Workspaces.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum PortfolioForecastingState { Draft, Active, Paused, Completed, Archived }
public sealed record PortfolioForecastingRecord(Guid Id, string Name, string Owner, PortfolioForecastingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record PortfolioForecastingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record PortfolioForecastingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record PortfolioForecastingQuery(string? SearchText, PortfolioForecastingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record PortfolioForecastingPage(IReadOnlyList<PortfolioForecastingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record PortfolioForecastingMutation(bool Succeeded, string Code, string Message, PortfolioForecastingRecord? Record, PortfolioForecastingEvent? Event);
public interface IPortfolioForecastingRepository
{
    ValueTask<PortfolioForecastingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<PortfolioForecastingPage> QueryAsync(PortfolioForecastingQuery query, CancellationToken cancellationToken);
    ValueTask<PortfolioForecastingMutation> SaveAsync(PortfolioForecastingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IPortfolioForecastingEventSink { ValueTask PublishAsync(PortfolioForecastingEvent domainEvent, CancellationToken cancellationToken); }