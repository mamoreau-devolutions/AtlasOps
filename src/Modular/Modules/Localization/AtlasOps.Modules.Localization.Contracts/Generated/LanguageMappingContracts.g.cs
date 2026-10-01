namespace AtlasOps.Modules.Localization.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LanguageMappingState { Draft, Active, Paused, Completed, Archived }
public sealed record LanguageMappingRecord(Guid Id, string Name, string Owner, LanguageMappingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LanguageMappingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LanguageMappingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LanguageMappingQuery(string? SearchText, LanguageMappingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LanguageMappingPage(IReadOnlyList<LanguageMappingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LanguageMappingMutation(bool Succeeded, string Code, string Message, LanguageMappingRecord? Record, LanguageMappingEvent? Event);
public interface ILanguageMappingRepository
{
    ValueTask<LanguageMappingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LanguageMappingPage> QueryAsync(LanguageMappingQuery query, CancellationToken cancellationToken);
    ValueTask<LanguageMappingMutation> SaveAsync(LanguageMappingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILanguageMappingEventSink { ValueTask PublishAsync(LanguageMappingEvent domainEvent, CancellationToken cancellationToken); }