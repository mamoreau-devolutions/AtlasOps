namespace AtlasOps.Modules.Localization.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LanguageValidationState { Draft, Active, Paused, Completed, Archived }
public sealed record LanguageValidationRecord(Guid Id, string Name, string Owner, LanguageValidationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LanguageValidationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LanguageValidationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LanguageValidationQuery(string? SearchText, LanguageValidationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LanguageValidationPage(IReadOnlyList<LanguageValidationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LanguageValidationMutation(bool Succeeded, string Code, string Message, LanguageValidationRecord? Record, LanguageValidationEvent? Event);
public interface ILanguageValidationRepository
{
    ValueTask<LanguageValidationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LanguageValidationPage> QueryAsync(LanguageValidationQuery query, CancellationToken cancellationToken);
    ValueTask<LanguageValidationMutation> SaveAsync(LanguageValidationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILanguageValidationEventSink { ValueTask PublishAsync(LanguageValidationEvent domainEvent, CancellationToken cancellationToken); }