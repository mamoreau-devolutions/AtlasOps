namespace AtlasOps.Modules.CloudEconomics.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ProviderReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record ProviderReportingRecord(Guid Id, string Name, string Owner, ProviderReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ProviderReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ProviderReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ProviderReportingQuery(string? SearchText, ProviderReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ProviderReportingPage(IReadOnlyList<ProviderReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ProviderReportingMutation(bool Succeeded, string Code, string Message, ProviderReportingRecord? Record, ProviderReportingEvent? Event);
public interface IProviderReportingRepository
{
    ValueTask<ProviderReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ProviderReportingPage> QueryAsync(ProviderReportingQuery query, CancellationToken cancellationToken);
    ValueTask<ProviderReportingMutation> SaveAsync(ProviderReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IProviderReportingEventSink { ValueTask PublishAsync(ProviderReportingEvent domainEvent, CancellationToken cancellationToken); }