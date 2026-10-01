namespace AtlasOps.Modules.Automation.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ApprovalDesignState { Draft, Active, Paused, Completed, Archived }
public sealed record ApprovalDesignRecord(Guid Id, string Name, string Owner, ApprovalDesignState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ApprovalDesignCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ApprovalDesignEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ApprovalDesignQuery(string? SearchText, ApprovalDesignState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ApprovalDesignPage(IReadOnlyList<ApprovalDesignRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ApprovalDesignMutation(bool Succeeded, string Code, string Message, ApprovalDesignRecord? Record, ApprovalDesignEvent? Event);
public interface IApprovalDesignRepository
{
    ValueTask<ApprovalDesignRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ApprovalDesignPage> QueryAsync(ApprovalDesignQuery query, CancellationToken cancellationToken);
    ValueTask<ApprovalDesignMutation> SaveAsync(ApprovalDesignRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IApprovalDesignEventSink { ValueTask PublishAsync(ApprovalDesignEvent domainEvent, CancellationToken cancellationToken); }