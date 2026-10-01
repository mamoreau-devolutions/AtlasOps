namespace AtlasOps.Modules.Identity.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TeamRevocationState { Draft, Active, Paused, Completed, Archived }
public sealed record TeamRevocationRecord(Guid Id, string Name, string Owner, TeamRevocationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TeamRevocationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TeamRevocationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TeamRevocationQuery(string? SearchText, TeamRevocationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TeamRevocationPage(IReadOnlyList<TeamRevocationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TeamRevocationMutation(bool Succeeded, string Code, string Message, TeamRevocationRecord? Record, TeamRevocationEvent? Event);
public interface ITeamRevocationRepository
{
    ValueTask<TeamRevocationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TeamRevocationPage> QueryAsync(TeamRevocationQuery query, CancellationToken cancellationToken);
    ValueTask<TeamRevocationMutation> SaveAsync(TeamRevocationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITeamRevocationEventSink { ValueTask PublishAsync(TeamRevocationEvent domainEvent, CancellationToken cancellationToken); }