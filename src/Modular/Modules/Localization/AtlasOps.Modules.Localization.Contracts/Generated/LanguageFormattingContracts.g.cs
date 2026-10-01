namespace AtlasOps.Modules.Localization.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LanguageFormattingState { Draft, Active, Paused, Completed, Archived }
public sealed record LanguageFormattingRecord(Guid Id, string Name, string Owner, LanguageFormattingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LanguageFormattingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LanguageFormattingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LanguageFormattingQuery(string? SearchText, LanguageFormattingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LanguageFormattingPage(IReadOnlyList<LanguageFormattingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LanguageFormattingMutation(bool Succeeded, string Code, string Message, LanguageFormattingRecord? Record, LanguageFormattingEvent? Event);
public interface ILanguageFormattingRepository
{
    ValueTask<LanguageFormattingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LanguageFormattingPage> QueryAsync(LanguageFormattingQuery query, CancellationToken cancellationToken);
    ValueTask<LanguageFormattingMutation> SaveAsync(LanguageFormattingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILanguageFormattingEventSink { ValueTask PublishAsync(LanguageFormattingEvent domainEvent, CancellationToken cancellationToken); }