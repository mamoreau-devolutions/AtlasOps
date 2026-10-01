namespace AtlasOps.Modules.Identity.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TeamProvisioningState { Draft, Active, Paused, Completed, Archived }
public sealed record TeamProvisioningRecord(Guid Id, string Name, string Owner, TeamProvisioningState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TeamProvisioningCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TeamProvisioningEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TeamProvisioningQuery(string? SearchText, TeamProvisioningState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TeamProvisioningPage(IReadOnlyList<TeamProvisioningRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TeamProvisioningMutation(bool Succeeded, string Code, string Message, TeamProvisioningRecord? Record, TeamProvisioningEvent? Event);
public interface ITeamProvisioningRepository
{
    ValueTask<TeamProvisioningRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TeamProvisioningPage> QueryAsync(TeamProvisioningQuery query, CancellationToken cancellationToken);
    ValueTask<TeamProvisioningMutation> SaveAsync(TeamProvisioningRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITeamProvisioningEventSink { ValueTask PublishAsync(TeamProvisioningEvent domainEvent, CancellationToken cancellationToken); }