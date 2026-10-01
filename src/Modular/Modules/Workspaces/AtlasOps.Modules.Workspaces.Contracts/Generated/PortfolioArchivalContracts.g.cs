namespace AtlasOps.Modules.Workspaces.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum PortfolioArchivalState { Draft, Active, Paused, Completed, Archived }
public sealed record PortfolioArchivalRecord(Guid Id, string Name, string Owner, PortfolioArchivalState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record PortfolioArchivalCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record PortfolioArchivalEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record PortfolioArchivalQuery(string? SearchText, PortfolioArchivalState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record PortfolioArchivalPage(IReadOnlyList<PortfolioArchivalRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record PortfolioArchivalMutation(bool Succeeded, string Code, string Message, PortfolioArchivalRecord? Record, PortfolioArchivalEvent? Event);
public interface IPortfolioArchivalRepository
{
    ValueTask<PortfolioArchivalRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<PortfolioArchivalPage> QueryAsync(PortfolioArchivalQuery query, CancellationToken cancellationToken);
    ValueTask<PortfolioArchivalMutation> SaveAsync(PortfolioArchivalRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IPortfolioArchivalEventSink { ValueTask PublishAsync(PortfolioArchivalEvent domainEvent, CancellationToken cancellationToken); }