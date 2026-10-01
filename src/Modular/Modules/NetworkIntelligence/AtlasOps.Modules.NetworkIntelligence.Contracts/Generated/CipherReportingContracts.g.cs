namespace AtlasOps.Modules.NetworkIntelligence.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum CipherReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record CipherReportingRecord(Guid Id, string Name, string Owner, CipherReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record CipherReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CipherReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record CipherReportingQuery(string? SearchText, CipherReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record CipherReportingPage(IReadOnlyList<CipherReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record CipherReportingMutation(bool Succeeded, string Code, string Message, CipherReportingRecord? Record, CipherReportingEvent? Event);
public interface ICipherReportingRepository
{
    ValueTask<CipherReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<CipherReportingPage> QueryAsync(CipherReportingQuery query, CancellationToken cancellationToken);
    ValueTask<CipherReportingMutation> SaveAsync(CipherReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ICipherReportingEventSink { ValueTask PublishAsync(CipherReportingEvent domainEvent, CancellationToken cancellationToken); }