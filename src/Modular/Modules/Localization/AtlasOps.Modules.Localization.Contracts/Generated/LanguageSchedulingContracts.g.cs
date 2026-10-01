namespace AtlasOps.Modules.Localization.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LanguageSchedulingState { Draft, Active, Paused, Completed, Archived }
public sealed record LanguageSchedulingRecord(Guid Id, string Name, string Owner, LanguageSchedulingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LanguageSchedulingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LanguageSchedulingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LanguageSchedulingQuery(string? SearchText, LanguageSchedulingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LanguageSchedulingPage(IReadOnlyList<LanguageSchedulingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LanguageSchedulingMutation(bool Succeeded, string Code, string Message, LanguageSchedulingRecord? Record, LanguageSchedulingEvent? Event);
public interface ILanguageSchedulingRepository
{
    ValueTask<LanguageSchedulingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LanguageSchedulingPage> QueryAsync(LanguageSchedulingQuery query, CancellationToken cancellationToken);
    ValueTask<LanguageSchedulingMutation> SaveAsync(LanguageSchedulingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILanguageSchedulingEventSink { ValueTask PublishAsync(LanguageSchedulingEvent domainEvent, CancellationToken cancellationToken); }