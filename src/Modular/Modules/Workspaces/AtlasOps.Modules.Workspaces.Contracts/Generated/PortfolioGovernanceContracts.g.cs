namespace AtlasOps.Modules.Workspaces.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum PortfolioGovernanceState { Draft, Active, Paused, Completed, Archived }
public sealed record PortfolioGovernanceRecord(Guid Id, string Name, string Owner, PortfolioGovernanceState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record PortfolioGovernanceCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record PortfolioGovernanceEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record PortfolioGovernanceQuery(string? SearchText, PortfolioGovernanceState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record PortfolioGovernancePage(IReadOnlyList<PortfolioGovernanceRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record PortfolioGovernanceMutation(bool Succeeded, string Code, string Message, PortfolioGovernanceRecord? Record, PortfolioGovernanceEvent? Event);
public interface IPortfolioGovernanceRepository
{
    ValueTask<PortfolioGovernanceRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<PortfolioGovernancePage> QueryAsync(PortfolioGovernanceQuery query, CancellationToken cancellationToken);
    ValueTask<PortfolioGovernanceMutation> SaveAsync(PortfolioGovernanceRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IPortfolioGovernanceEventSink { ValueTask PublishAsync(PortfolioGovernanceEvent domainEvent, CancellationToken cancellationToken); }