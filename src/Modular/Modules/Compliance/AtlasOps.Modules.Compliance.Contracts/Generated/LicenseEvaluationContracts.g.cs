namespace AtlasOps.Modules.Compliance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LicenseEvaluationState { Draft, Active, Paused, Completed, Archived }
public sealed record LicenseEvaluationRecord(Guid Id, string Name, string Owner, LicenseEvaluationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LicenseEvaluationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LicenseEvaluationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LicenseEvaluationQuery(string? SearchText, LicenseEvaluationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LicenseEvaluationPage(IReadOnlyList<LicenseEvaluationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LicenseEvaluationMutation(bool Succeeded, string Code, string Message, LicenseEvaluationRecord? Record, LicenseEvaluationEvent? Event);
public interface ILicenseEvaluationRepository
{
    ValueTask<LicenseEvaluationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LicenseEvaluationPage> QueryAsync(LicenseEvaluationQuery query, CancellationToken cancellationToken);
    ValueTask<LicenseEvaluationMutation> SaveAsync(LicenseEvaluationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILicenseEvaluationEventSink { ValueTask PublishAsync(LicenseEvaluationEvent domainEvent, CancellationToken cancellationToken); }