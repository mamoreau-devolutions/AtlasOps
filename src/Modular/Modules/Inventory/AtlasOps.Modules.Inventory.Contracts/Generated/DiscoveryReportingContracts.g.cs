namespace AtlasOps.Modules.Inventory.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum DiscoveryReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record DiscoveryReportingRecord(Guid Id, string Name, string Owner, DiscoveryReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record DiscoveryReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record DiscoveryReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record DiscoveryReportingQuery(string? SearchText, DiscoveryReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record DiscoveryReportingPage(IReadOnlyList<DiscoveryReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record DiscoveryReportingMutation(bool Succeeded, string Code, string Message, DiscoveryReportingRecord? Record, DiscoveryReportingEvent? Event);
public interface IDiscoveryReportingRepository
{
    ValueTask<DiscoveryReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<DiscoveryReportingPage> QueryAsync(DiscoveryReportingQuery query, CancellationToken cancellationToken);
    ValueTask<DiscoveryReportingMutation> SaveAsync(DiscoveryReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IDiscoveryReportingEventSink { ValueTask PublishAsync(DiscoveryReportingEvent domainEvent, CancellationToken cancellationToken); }