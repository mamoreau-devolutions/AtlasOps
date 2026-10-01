namespace AtlasOps.Modules.Workspaces.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum PortfolioCollaborationState { Draft, Active, Paused, Completed, Archived }
public sealed record PortfolioCollaborationRecord(Guid Id, string Name, string Owner, PortfolioCollaborationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record PortfolioCollaborationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record PortfolioCollaborationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record PortfolioCollaborationQuery(string? SearchText, PortfolioCollaborationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record PortfolioCollaborationPage(IReadOnlyList<PortfolioCollaborationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record PortfolioCollaborationMutation(bool Succeeded, string Code, string Message, PortfolioCollaborationRecord? Record, PortfolioCollaborationEvent? Event);
public interface IPortfolioCollaborationRepository
{
    ValueTask<PortfolioCollaborationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<PortfolioCollaborationPage> QueryAsync(PortfolioCollaborationQuery query, CancellationToken cancellationToken);
    ValueTask<PortfolioCollaborationMutation> SaveAsync(PortfolioCollaborationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IPortfolioCollaborationEventSink { ValueTask PublishAsync(PortfolioCollaborationEvent domainEvent, CancellationToken cancellationToken); }