namespace AtlasOps.Modules.Localization.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TerritoryValidationState { Draft, Active, Paused, Completed, Archived }
public sealed record TerritoryValidationRecord(Guid Id, string Name, string Owner, TerritoryValidationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TerritoryValidationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TerritoryValidationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TerritoryValidationQuery(string? SearchText, TerritoryValidationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TerritoryValidationPage(IReadOnlyList<TerritoryValidationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TerritoryValidationMutation(bool Succeeded, string Code, string Message, TerritoryValidationRecord? Record, TerritoryValidationEvent? Event);
public interface ITerritoryValidationRepository
{
    ValueTask<TerritoryValidationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TerritoryValidationPage> QueryAsync(TerritoryValidationQuery query, CancellationToken cancellationToken);
    ValueTask<TerritoryValidationMutation> SaveAsync(TerritoryValidationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITerritoryValidationEventSink { ValueTask PublishAsync(TerritoryValidationEvent domainEvent, CancellationToken cancellationToken); }