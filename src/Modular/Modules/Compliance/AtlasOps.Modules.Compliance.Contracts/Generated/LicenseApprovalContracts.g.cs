namespace AtlasOps.Modules.Compliance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LicenseApprovalState { Draft, Active, Paused, Completed, Archived }
public sealed record LicenseApprovalRecord(Guid Id, string Name, string Owner, LicenseApprovalState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LicenseApprovalCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LicenseApprovalEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LicenseApprovalQuery(string? SearchText, LicenseApprovalState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LicenseApprovalPage(IReadOnlyList<LicenseApprovalRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LicenseApprovalMutation(bool Succeeded, string Code, string Message, LicenseApprovalRecord? Record, LicenseApprovalEvent? Event);
public interface ILicenseApprovalRepository
{
    ValueTask<LicenseApprovalRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LicenseApprovalPage> QueryAsync(LicenseApprovalQuery query, CancellationToken cancellationToken);
    ValueTask<LicenseApprovalMutation> SaveAsync(LicenseApprovalRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILicenseApprovalEventSink { ValueTask PublishAsync(LicenseApprovalEvent domainEvent, CancellationToken cancellationToken); }