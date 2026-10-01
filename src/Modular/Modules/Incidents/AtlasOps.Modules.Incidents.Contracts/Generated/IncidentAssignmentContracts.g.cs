namespace AtlasOps.Modules.Incidents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum IncidentAssignmentState { Draft, Active, Paused, Completed, Archived }
public sealed record IncidentAssignmentRecord(Guid Id, string Name, string Owner, IncidentAssignmentState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record IncidentAssignmentCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record IncidentAssignmentEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record IncidentAssignmentQuery(string? SearchText, IncidentAssignmentState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record IncidentAssignmentPage(IReadOnlyList<IncidentAssignmentRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record IncidentAssignmentMutation(bool Succeeded, string Code, string Message, IncidentAssignmentRecord? Record, IncidentAssignmentEvent? Event);
public interface IIncidentAssignmentRepository
{
    ValueTask<IncidentAssignmentRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<IncidentAssignmentPage> QueryAsync(IncidentAssignmentQuery query, CancellationToken cancellationToken);
    ValueTask<IncidentAssignmentMutation> SaveAsync(IncidentAssignmentRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IIncidentAssignmentEventSink { ValueTask PublishAsync(IncidentAssignmentEvent domainEvent, CancellationToken cancellationToken); }