namespace AtlasOps.Modules.Documents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TemplateApprovalState { Draft, Active, Paused, Completed, Archived }
public sealed record TemplateApprovalRecord(Guid Id, string Name, string Owner, TemplateApprovalState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TemplateApprovalCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TemplateApprovalEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TemplateApprovalQuery(string? SearchText, TemplateApprovalState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TemplateApprovalPage(IReadOnlyList<TemplateApprovalRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TemplateApprovalMutation(bool Succeeded, string Code, string Message, TemplateApprovalRecord? Record, TemplateApprovalEvent? Event);
public interface ITemplateApprovalRepository
{
    ValueTask<TemplateApprovalRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TemplateApprovalPage> QueryAsync(TemplateApprovalQuery query, CancellationToken cancellationToken);
    ValueTask<TemplateApprovalMutation> SaveAsync(TemplateApprovalRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITemplateApprovalEventSink { ValueTask PublishAsync(TemplateApprovalEvent domainEvent, CancellationToken cancellationToken); }