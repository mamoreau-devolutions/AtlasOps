namespace AtlasOps.Integrations.SecureShell.Core;

using System.Text.RegularExpressions;

using AtlasOps.Integrations.SecureShell.Contracts;

public sealed partial class SecureShellService
{
    public SecureCommandValidation Validate(
        SecureShellEndpoint endpoint,
        SecureCommandPlan command,
        HostKeyRecord? hostKey,
        DateTimeOffset now)
    {
        List<string> diagnostics = [];

        if (string.IsNullOrWhiteSpace(endpoint.Host) || endpoint.Port is < 1 or > 65_535)
        {
            diagnostics.Add("A valid host and port are required.");
        }

        if (string.IsNullOrWhiteSpace(endpoint.UserName) ||
            string.IsNullOrWhiteSpace(endpoint.CredentialReference))
        {
            diagnostics.Add("A user name and credential reference are required.");
        }

        if (string.IsNullOrWhiteSpace(command.Executable) ||
            ShellMetacharacters().IsMatch(command.Executable))
        {
            diagnostics.Add("The executable cannot contain shell metacharacters.");
        }

        for (int index = 0; index < command.Arguments.Count; index++)
        {
            string argument = command.Arguments[index];
            if (argument.Contains('\0'))
            {
                diagnostics.Add($"Argument {index} contains a null character.");
            }
        }

        if (hostKey is null)
        {
            diagnostics.Add("The host key has not been accepted.");
        }
        else
        {
            if (!string.Equals(hostKey.Host, endpoint.Host, StringComparison.OrdinalIgnoreCase) ||
                hostKey.Port != endpoint.Port)
            {
                diagnostics.Add("The host key is not bound to the requested endpoint.");
            }

            if (hostKey.ExpiresAt is not null && hostKey.ExpiresAt <= now)
            {
                diagnostics.Add("The accepted host key has expired.");
            }
        }

        return new SecureCommandValidation(diagnostics.Count == 0, diagnostics);
    }

    public IReadOnlyList<string> RedactArguments(
        IReadOnlyList<string> arguments,
        IReadOnlySet<int> sensitiveIndexes)
    {
        return arguments
            .Select((argument, index) => sensitiveIndexes.Contains(index) ? "[REDACTED]" : argument)
            .ToArray();
    }

    [GeneratedRegex("[;&|`$><\\r\\n]")]
    private static partial Regex ShellMetacharacters();
}
