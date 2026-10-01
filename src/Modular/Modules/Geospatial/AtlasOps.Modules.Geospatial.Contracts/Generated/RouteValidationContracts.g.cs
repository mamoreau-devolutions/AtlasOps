namespace AtlasOps.Modules.Geospatial.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RouteValidationState { Draft, Active, Paused, Completed, Archived }
public sealed record RouteValidationRecord(Guid Id, string Name, string Owner, RouteValidationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RouteValidationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RouteValidationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RouteValidationQuery(string? SearchText, RouteValidationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RouteValidationPage(IReadOnlyList<RouteValidationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RouteValidationMutation(bool Succeeded, string Code, string Message, RouteValidationRecord? Record, RouteValidationEvent? Event);
public interface IRouteValidationRepository
{
    ValueTask<RouteValidationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RouteValidationPage> QueryAsync(RouteValidationQuery query, CancellationToken cancellationToken);
    ValueTask<RouteValidationMutation> SaveAsync(RouteValidationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRouteValidationEventSink { ValueTask PublishAsync(RouteValidationEvent domainEvent, CancellationToken cancellationToken); }