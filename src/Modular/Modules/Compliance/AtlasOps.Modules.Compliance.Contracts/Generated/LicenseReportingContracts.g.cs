namespace AtlasOps.Modules.Compliance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LicenseReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record LicenseReportingRecord(Guid Id, string Name, string Owner, LicenseReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LicenseReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LicenseReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LicenseReportingQuery(string? SearchText, LicenseReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LicenseReportingPage(IReadOnlyList<LicenseReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LicenseReportingMutation(bool Succeeded, string Code, string Message, LicenseReportingRecord? Record, LicenseReportingEvent? Event);
public interface ILicenseReportingRepository
{
    ValueTask<LicenseReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LicenseReportingPage> QueryAsync(LicenseReportingQuery query, CancellationToken cancellationToken);
    ValueTask<LicenseReportingMutation> SaveAsync(LicenseReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILicenseReportingEventSink { ValueTask PublishAsync(LicenseReportingEvent domainEvent, CancellationToken cancellationToken); }