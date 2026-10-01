namespace AtlasOps.Integrations.Tests;

using AtlasOps.Integrations.SecureShell.Contracts;
using AtlasOps.Integrations.SecureShell.Core;

[TestClass]
public sealed class SecureShellServiceTests
{
    private static readonly DateTimeOffset Now = new(2026, 2, 3, 4, 5, 6, TimeSpan.Zero);

    [TestMethod]
    public void Validate_ValidEndpointCommandAndHostKey_IsValidAtPortBoundaries()
    {
        SecureShellService service = new();
        SecureShellEndpoint endpoint = Endpoint(1);
        HostKeyRecord key = new("HOST", 1, "ed25519", "fingerprint", Now, Now.AddTicks(1));

        SecureCommandValidation result = service.Validate(endpoint, Command("tool"), key, Now);

        Assert.IsTrue(result.Valid);
        Assert.IsEmpty(result.Diagnostics);

        SecureShellEndpoint upperEndpoint = Endpoint(65_535);
        HostKeyRecord upperKey = key with { Port = 65_535 };
        Assert.IsTrue(service.Validate(upperEndpoint, Command("tool"), upperKey, Now).Valid);
    }

    [TestMethod]
    public void Validate_InvalidIdentityExecutableArgumentAndMissingKey_ReturnsAllDiagnostics()
    {
        SecureShellService service = new();
        SecureShellEndpoint endpoint = new("", 65_536, "", "", TimeSpan.FromSeconds(1));
        SecureCommandPlan command = Command("tool;rm") with { Arguments = ["ok", "bad\0value"] };

        SecureCommandValidation result = service.Validate(endpoint, command, null, Now);

        Assert.IsFalse(result.Valid);
        CollectionAssert.AreEqual(
            new[]
            {
                "A valid host and port are required.",
                "A user name and credential reference are required.",
                "The executable cannot contain shell metacharacters.",
                "Argument 1 contains a null character.",
                "The host key has not been accepted.",
            },
            result.Diagnostics.ToArray());
    }

    [TestMethod]
    public void Validate_MismatchedAndExpiredHostKey_ReturnsBothDiagnostics()
    {
        SecureShellService service = new();
        SecureShellEndpoint endpoint = Endpoint(22);
        HostKeyRecord key = new("other", 23, "rsa", "fingerprint", Now, Now);

        SecureCommandValidation result = service.Validate(endpoint, Command("tool"), key, Now);

        Assert.Contains("The host key is not bound to the requested endpoint.", result.Diagnostics);
        Assert.Contains("The accepted host key has expired.", result.Diagnostics);
    }

    [TestMethod]
    public void RedactArguments_ReplacesOnlySensitiveIndexesAndPreservesOrder()
    {
        SecureShellService service = new();
        string[] input = ["command", "secret", "safe"];

        IReadOnlyList<string> result = service.RedactArguments(input, new HashSet<int>([1]));

        CollectionAssert.AreEqual(new[] { "command", "[REDACTED]", "safe" }, result.ToArray());
        CollectionAssert.AreEqual(new[] { "command", "secret", "safe" }, input);
    }

    private static SecureShellEndpoint Endpoint(int port)
    {
        return new SecureShellEndpoint("host", port, "user", "credential", TimeSpan.FromSeconds(30));
    }

    private static SecureCommandPlan Command(string executable)
    {
        return new SecureCommandPlan(executable, [], new Dictionary<string, string>(), "/", false);
    }
}
