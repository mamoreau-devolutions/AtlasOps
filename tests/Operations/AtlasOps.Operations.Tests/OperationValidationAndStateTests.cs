namespace AtlasOps.Operations.Tests;

using AtlasOps.Operations.Contracts;
using AtlasOps.Operations.Runtime;

[TestClass]
public sealed class OperationValidationAndStateTests
{
    [TestMethod]
    public void ValidateDescriptor_AtInclusiveBoundaries_IsValid()
    {
        OperationDescriptor descriptor = new(
            "sync",
            "Synchronize",
            "git",
            true,
            true,
            1,
            100,
            TimeSpan.FromMilliseconds(100));

        OperationValidationResult result = OperationValidator.Validate(descriptor);

        Assert.IsTrue(result.Valid);
        Assert.IsEmpty(result.Diagnostics);
    }

    [TestMethod]
    public void ValidateDescriptor_AboveAndBelowBoundaries_ReturnsEveryDiagnostic()
    {
        OperationDescriptor descriptor = new(
            " ",
            "",
            "\t",
            false,
            false,
            1_025,
            0,
            TimeSpan.FromHours(24).Add(TimeSpan.FromTicks(1)));

        OperationValidationResult result = OperationValidator.Validate(descriptor);

        Assert.IsFalse(result.Valid);
        CollectionAssert.AreEqual(
            new[]
            {
                "Operation ID is required.",
                "Display name is required.",
                "Provider family is required.",
                "Maximum concurrency must be between 1 and 1,024.",
                "Maximum attempts must be between 1 and 100.",
                "Timeout must be between 100 milliseconds and 24 hours.",
            },
            result.Diagnostics.ToArray());
    }

    [TestMethod]
    [DataRow(0, 1, 100)]
    [DataRow(1_025, 1, 100)]
    [DataRow(1, 0, 100)]
    [DataRow(1, 101, 100)]
    [DataRow(1, 1, 99)]
    public void ValidateDescriptor_ImmediatelyOutsideEachNumericBoundary_IsInvalid(
        int maximumConcurrency,
        int maximumAttempts,
        int timeoutMilliseconds)
    {
        OperationDescriptor descriptor = new(
            "sync",
            "Synchronize",
            "provider",
            false,
            false,
            maximumConcurrency,
            maximumAttempts,
            TimeSpan.FromMilliseconds(timeoutMilliseconds));

        OperationValidationResult result = OperationValidator.Validate(descriptor);

        Assert.IsFalse(result.Valid);
        Assert.HasCount(1, result.Diagnostics);
    }

    [TestMethod]
    public void ValidateEnvelope_InvalidIdentityTimestampKeyAndOversizedValue_ReturnsConcreteDiagnostics()
    {
        Dictionary<string, string> parameters = new()
        {
            [" "] = new string('x', 1_000_001),
        };
        OperationEnvelope envelope = new(
            Guid.Empty,
            " ",
            "",
            default,
            null,
            parameters);

        OperationValidationResult result = OperationValidator.Validate(envelope);

        Assert.IsFalse(result.Valid);
        Assert.HasCount(6, result.Diagnostics);
        CollectionAssert.Contains(result.Diagnostics.ToArray(), "Operation ID cannot be empty.");
        CollectionAssert.Contains(result.Diagnostics.ToArray(), "Parameter key is required.");
        CollectionAssert.Contains(
            result.Diagnostics.ToArray(),
            "Parameter ' ' exceeds the one-million-character limit.");
    }

    [TestMethod]
    public void ValidateEnvelope_ValueAtSizeBoundary_IsValid()
    {
        OperationEnvelope envelope = OperationTestData.Envelope() with
        {
            Parameters = new Dictionary<string, string>
            {
                ["payload"] = new string('x', 1_000_000),
            },
        };

        OperationValidationResult result = OperationValidator.Validate(envelope);

        Assert.IsTrue(result.Valid);
        Assert.IsEmpty(result.Diagnostics);
    }

    [TestMethod]
    public void Transition_PendingToLeasedThenRunning_UpdatesRevisionAndOnlyRunningAttempt()
    {
        DurableJob job = OperationTestData.Job();

        JobMutationResult leased = OperationStateMachine.Transition(
            job,
            DurableJobStatus.Leased,
            OperationTestData.Now.AddMinutes(1),
            OperationTestData.Now.AddMinutes(2),
            "leased");
        JobMutationResult running = OperationStateMachine.Transition(
            leased.Job!,
            DurableJobStatus.Running,
            OperationTestData.Now.AddMinutes(2),
            OperationTestData.Now.AddMinutes(3),
            "running");

        Assert.IsTrue(leased.Succeeded);
        Assert.AreEqual(0, leased.Job!.Attempt);
        Assert.AreEqual(1L, leased.Job.Revision);
        Assert.IsTrue(running.Succeeded);
        Assert.AreEqual(DurableJobStatus.Running, running.Job!.Status);
        Assert.AreEqual(1, running.Job.Attempt);
        Assert.AreEqual(2L, running.Job.Revision);
        Assert.AreEqual("running", running.Job.LastDiagnostic);
    }

    [TestMethod]
    public void Transition_TerminalOrOtherwiseInvalid_PreservesJob()
    {
        DurableJob succeeded = OperationTestData.Job(DurableJobStatus.Succeeded);

        JobMutationResult result = OperationStateMachine.Transition(
            succeeded,
            DurableJobStatus.Pending,
            OperationTestData.Now,
            OperationTestData.Now,
            null);

        Assert.IsFalse(result.Succeeded);
        Assert.AreSame(succeeded, result.Job);
        Assert.AreEqual("Transition from Succeeded to Pending is not allowed.", result.Diagnostic);
        Assert.IsFalse(OperationStateMachine.CanTransition(DurableJobStatus.Pending, DurableJobStatus.Succeeded));
    }

    [TestMethod]
    public void Transition_RunningWouldExceedMaximumAttempts_IsRejected()
    {
        DurableJob leased = OperationTestData.Job(
            DurableJobStatus.Leased,
            attempt: 1,
            maximumAttempts: 1);

        JobMutationResult result = OperationStateMachine.Transition(
            leased,
            DurableJobStatus.Running,
            OperationTestData.Now,
            OperationTestData.Now,
            null);

        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual("Maximum attempts have been exhausted.", result.Diagnostic);
        Assert.AreEqual(1, result.Job!.Attempt);
        Assert.AreEqual(0L, result.Job.Revision);
    }

    [TestMethod]
    [DataRow(DurableJobStatus.Pending, DurableJobStatus.Cancelled)]
    [DataRow(DurableJobStatus.Leased, DurableJobStatus.Pending)]
    [DataRow(DurableJobStatus.Leased, DurableJobStatus.Cancelled)]
    [DataRow(DurableJobStatus.Running, DurableJobStatus.Succeeded)]
    [DataRow(DurableJobStatus.Running, DurableJobStatus.WaitingForRetry)]
    [DataRow(DurableJobStatus.Running, DurableJobStatus.Failed)]
    [DataRow(DurableJobStatus.Running, DurableJobStatus.Cancelled)]
    [DataRow(DurableJobStatus.Running, DurableJobStatus.DeadLettered)]
    [DataRow(DurableJobStatus.WaitingForRetry, DurableJobStatus.Leased)]
    [DataRow(DurableJobStatus.WaitingForRetry, DurableJobStatus.Cancelled)]
    [DataRow(DurableJobStatus.WaitingForRetry, DurableJobStatus.DeadLettered)]
    [DataRow(DurableJobStatus.Failed, DurableJobStatus.WaitingForRetry)]
    [DataRow(DurableJobStatus.Failed, DurableJobStatus.DeadLettered)]
    public void CanTransition_EveryRemainingDocumentedTransition_IsAllowed(
        DurableJobStatus current,
        DurableJobStatus next)
    {
        Assert.IsTrue(OperationStateMachine.CanTransition(current, next));
    }
}
