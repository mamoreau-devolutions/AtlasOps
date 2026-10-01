namespace AtlasOps.Features.Automation.AutomationCredential;

public sealed record UpdateAutomationCredentialCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);