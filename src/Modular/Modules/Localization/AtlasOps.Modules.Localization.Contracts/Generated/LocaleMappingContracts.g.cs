namespace AtlasOps.Modules.Localization.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LocaleMappingState { Draft, Active, Paused, Completed, Archived }
public sealed record LocaleMappingRecord(Guid Id, string Name, string Owner, LocaleMappingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LocaleMappingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LocaleMappingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LocaleMappingQuery(string? SearchText, LocaleMappingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LocaleMappingPage(IReadOnlyList<LocaleMappingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LocaleMappingMutation(bool Succeeded, string Code, string Message, LocaleMappingRecord? Record, LocaleMappingEvent? Event);
public interface ILocaleMappingRepository
{
    ValueTask<LocaleMappingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LocaleMappingPage> QueryAsync(LocaleMappingQuery query, CancellationToken cancellationToken);
    ValueTask<LocaleMappingMutation> SaveAsync(LocaleMappingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILocaleMappingEventSink { ValueTask PublishAsync(LocaleMappingEvent domainEvent, CancellationToken cancellationToken); }