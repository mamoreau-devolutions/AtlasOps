namespace AtlasOps.Modules.Localization.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LocaleValidationState { Draft, Active, Paused, Completed, Archived }
public sealed record LocaleValidationRecord(Guid Id, string Name, string Owner, LocaleValidationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LocaleValidationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LocaleValidationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LocaleValidationQuery(string? SearchText, LocaleValidationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LocaleValidationPage(IReadOnlyList<LocaleValidationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LocaleValidationMutation(bool Succeeded, string Code, string Message, LocaleValidationRecord? Record, LocaleValidationEvent? Event);
public interface ILocaleValidationRepository
{
    ValueTask<LocaleValidationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LocaleValidationPage> QueryAsync(LocaleValidationQuery query, CancellationToken cancellationToken);
    ValueTask<LocaleValidationMutation> SaveAsync(LocaleValidationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILocaleValidationEventSink { ValueTask PublishAsync(LocaleValidationEvent domainEvent, CancellationToken cancellationToken); }