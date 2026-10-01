namespace AtlasOps.Modules.Workspaces.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum PortfolioLifecycleState { Draft, Active, Paused, Completed, Archived }
public sealed record PortfolioLifecycleRecord(Guid Id, string Name, string Owner, PortfolioLifecycleState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record PortfolioLifecycleCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record PortfolioLifecycleEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record PortfolioLifecycleQuery(string? SearchText, PortfolioLifecycleState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record PortfolioLifecyclePage(IReadOnlyList<PortfolioLifecycleRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record PortfolioLifecycleMutation(bool Succeeded, string Code, string Message, PortfolioLifecycleRecord? Record, PortfolioLifecycleEvent? Event);
public interface IPortfolioLifecycleRepository
{
    ValueTask<PortfolioLifecycleRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<PortfolioLifecyclePage> QueryAsync(PortfolioLifecycleQuery query, CancellationToken cancellationToken);
    ValueTask<PortfolioLifecycleMutation> SaveAsync(PortfolioLifecycleRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IPortfolioLifecycleEventSink { ValueTask PublishAsync(PortfolioLifecycleEvent domainEvent, CancellationToken cancellationToken); }