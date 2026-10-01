namespace AtlasOps.Modules.Localization.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum CurrencyValidationState { Draft, Active, Paused, Completed, Archived }
public sealed record CurrencyValidationRecord(Guid Id, string Name, string Owner, CurrencyValidationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record CurrencyValidationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CurrencyValidationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record CurrencyValidationQuery(string? SearchText, CurrencyValidationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record CurrencyValidationPage(IReadOnlyList<CurrencyValidationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record CurrencyValidationMutation(bool Succeeded, string Code, string Message, CurrencyValidationRecord? Record, CurrencyValidationEvent? Event);
public interface ICurrencyValidationRepository
{
    ValueTask<CurrencyValidationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<CurrencyValidationPage> QueryAsync(CurrencyValidationQuery query, CancellationToken cancellationToken);
    ValueTask<CurrencyValidationMutation> SaveAsync(CurrencyValidationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ICurrencyValidationEventSink { ValueTask PublishAsync(CurrencyValidationEvent domainEvent, CancellationToken cancellationToken); }