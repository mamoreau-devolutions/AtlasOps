namespace AtlasOps.Modules.Automation.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum WorkflowDesignState { Draft, Active, Paused, Completed, Archived }
public sealed record WorkflowDesignRecord(Guid Id, string Name, string Owner, WorkflowDesignState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record WorkflowDesignCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record WorkflowDesignEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record WorkflowDesignQuery(string? SearchText, WorkflowDesignState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record WorkflowDesignPage(IReadOnlyList<WorkflowDesignRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record WorkflowDesignMutation(bool Succeeded, string Code, string Message, WorkflowDesignRecord? Record, WorkflowDesignEvent? Event);
public interface IWorkflowDesignRepository
{
    ValueTask<WorkflowDesignRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<WorkflowDesignPage> QueryAsync(WorkflowDesignQuery query, CancellationToken cancellationToken);
    ValueTask<WorkflowDesignMutation> SaveAsync(WorkflowDesignRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IWorkflowDesignEventSink { ValueTask PublishAsync(WorkflowDesignEvent domainEvent, CancellationToken cancellationToken); }