namespace AtlasOps.Modules.Workspaces.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum PortfolioReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record PortfolioReportingRecord(Guid Id, string Name, string Owner, PortfolioReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record PortfolioReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record PortfolioReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record PortfolioReportingQuery(string? SearchText, PortfolioReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record PortfolioReportingPage(IReadOnlyList<PortfolioReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record PortfolioReportingMutation(bool Succeeded, string Code, string Message, PortfolioReportingRecord? Record, PortfolioReportingEvent? Event);
public interface IPortfolioReportingRepository
{
    ValueTask<PortfolioReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<PortfolioReportingPage> QueryAsync(PortfolioReportingQuery query, CancellationToken cancellationToken);
    ValueTask<PortfolioReportingMutation> SaveAsync(PortfolioReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IPortfolioReportingEventSink { ValueTask PublishAsync(PortfolioReportingEvent domainEvent, CancellationToken cancellationToken); }