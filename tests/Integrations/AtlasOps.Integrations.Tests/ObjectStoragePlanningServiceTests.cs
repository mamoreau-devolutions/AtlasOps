namespace AtlasOps.Integrations.Tests;

using System.Security.Cryptography;

using AtlasOps.Integrations.ObjectStorage.Contracts;
using AtlasOps.Integrations.ObjectStorage.Core;

[TestClass]
public sealed class ObjectStoragePlanningServiceTests
{
    [TestMethod]
    public void CreatePlan_MultipartContentProducesSequentialOffsetsAndHashes()
    {
        ObjectStoragePlanningService service = new();
        byte[] content = [1, 2, 3, 4, 5];

        MultipartUploadPlan result = service.CreatePlan("bucket", "key", "application/octet-stream", content, 2);

        Assert.AreEqual(5L, result.ContentLength);
        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, result.Parts.Select(static part => part.Number).ToArray());
        CollectionAssert.AreEqual(new long[] { 0, 2, 4 }, result.Parts.Select(static part => part.Offset).ToArray());
        CollectionAssert.AreEqual(new[] { 2, 2, 1 }, result.Parts.Select(static part => part.Length).ToArray());
        Assert.AreEqual(Convert.ToHexString(SHA256.HashData(content)), result.ContentSha256);
        Assert.AreEqual(
            Convert.ToHexString(SHA256.HashData(content.AsSpan(0, 2))),
            result.Parts[0].Sha256);
    }

    [TestMethod]
    public void CreatePlan_EmptyContentProducesNoPartsAndEmptyHash()
    {
        ObjectStoragePlanningService service = new();

        MultipartUploadPlan result = service.CreatePlan("bucket", "key", "text/plain", [], 2);

        Assert.IsEmpty(result.Parts);
        Assert.AreEqual(0L, result.ContentLength);
        Assert.AreEqual(Convert.ToHexString(SHA256.HashData([])), result.ContentSha256);
    }

    [TestMethod]
    public void Validate_ValidContiguousPlan_IsValid()
    {
        ObjectStoragePlanningService service = new();
        MultipartUploadPlan plan = new(
            "bucket",
            "folder/file",
            3,
            "text/plain",
            [new ObjectPart(1, 0, 3, "HASH")],
            "TOTAL");

        ObjectPlanValidation result = service.Validate(plan);

        Assert.IsTrue(result.Valid);
        Assert.IsEmpty(result.Diagnostics);
    }

    [TestMethod]
    public void Validate_PathAndPartInvariantViolations_ReturnEveryDiagnostic()
    {
        ObjectStoragePlanningService service = new();
        MultipartUploadPlan plan = new(
            "",
            "../secret",
            -1,
            "text/plain",
            [new ObjectPart(2, 5, 0, "")],
            "TOTAL");

        ObjectPlanValidation result = service.Validate(plan);

        Assert.IsFalse(result.Valid);
        Assert.Contains("Bucket is required.", result.Diagnostics);
        Assert.Contains("Object key must be relative and cannot contain traversal segments.", result.Diagnostics);
        Assert.Contains("Content length cannot be negative.", result.Diagnostics);
        Assert.Contains("Expected part 1 but found 2.", result.Diagnostics);
        Assert.Contains("Part 2 has an invalid offset.", result.Diagnostics);
        Assert.Contains("Part 2 has invalid length or hash.", result.Diagnostics);
        Assert.Contains("Part lengths do not equal the total content length.", result.Diagnostics);
    }

    [TestMethod]
    public void Validate_RootedObjectKey_IsRejected()
    {
        ObjectStoragePlanningService service = new();
        MultipartUploadPlan plan = new(
            "bucket",
            "/rooted",
            0,
            "text/plain",
            [],
            "HASH");

        ObjectPlanValidation result = service.Validate(plan);

        Assert.IsFalse(result.Valid);
        CollectionAssert.AreEqual(
            new[] { "Object key must be relative and cannot contain traversal segments." },
            result.Diagnostics.ToArray());
    }
}
