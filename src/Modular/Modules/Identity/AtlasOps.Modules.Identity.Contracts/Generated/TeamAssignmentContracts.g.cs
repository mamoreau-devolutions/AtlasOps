namespace AtlasOps.Modules.Identity.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TeamAssignmentState { Draft, Active, Paused, Completed, Archived }
public sealed record TeamAssignmentRecord(Guid Id, string Name, string Owner, TeamAssignmentState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TeamAssignmentCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TeamAssignmentEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TeamAssignmentQuery(string? SearchText, TeamAssignmentState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TeamAssignmentPage(IReadOnlyList<TeamAssignmentRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TeamAssignmentMutation(bool Succeeded, string Code, string Message, TeamAssignmentRecord? Record, TeamAssignmentEvent? Event);
public interface ITeamAssignmentRepository
{
    ValueTask<TeamAssignmentRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TeamAssignmentPage> QueryAsync(TeamAssignmentQuery query, CancellationToken cancellationToken);
    ValueTask<TeamAssignmentMutation> SaveAsync(TeamAssignmentRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITeamAssignmentEventSink { ValueTask PublishAsync(TeamAssignmentEvent domainEvent, CancellationToken cancellationToken); }