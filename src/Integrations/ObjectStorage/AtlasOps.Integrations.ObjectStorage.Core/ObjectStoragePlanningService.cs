namespace AtlasOps.Integrations.ObjectStorage.Core;

using System.Security.Cryptography;

using AtlasOps.Integrations.ObjectStorage.Contracts;

public sealed class ObjectStoragePlanningService
{
    public MultipartUploadPlan CreatePlan(
        string bucket,
        string objectKey,
        string contentType,
        byte[] content,
        int partSize)
    {
        List<ObjectPart> parts = [];
        int offset = 0;
        int number = 1;
        while (offset < content.Length)
        {
            int length = Math.Min(partSize, content.Length - offset);
            string hash = Convert.ToHexString(SHA256.HashData(content.AsSpan(offset, length)));
            parts.Add(new ObjectPart(number, offset, length, hash));
            offset += length;
            number++;
        }

        return new MultipartUploadPlan(
            bucket,
            objectKey,
            content.LongLength,
            contentType,
            parts,
            Convert.ToHexString(SHA256.HashData(content)));
    }

    public ObjectPlanValidation Validate(MultipartUploadPlan plan)
    {
        List<string> diagnostics = [];

        if (string.IsNullOrWhiteSpace(plan.Bucket))
        {
            diagnostics.Add("Bucket is required.");
        }

        if (string.IsNullOrWhiteSpace(plan.ObjectKey) ||
            plan.ObjectKey.StartsWith("/", StringComparison.Ordinal) ||
            plan.ObjectKey.Split('/').Any(static segment => segment is "." or ".."))
        {
            diagnostics.Add("Object key must be relative and cannot contain traversal segments.");
        }

        if (plan.ContentLength < 0)
        {
            diagnostics.Add("Content length cannot be negative.");
        }

        long expectedOffset = 0;
        int expectedNumber = 1;
        foreach (ObjectPart part in plan.Parts.OrderBy(static item => item.Number))
        {
            if (part.Number != expectedNumber)
            {
                diagnostics.Add($"Expected part {expectedNumber} but found {part.Number}.");
            }

            if (part.Offset != expectedOffset)
            {
                diagnostics.Add($"Part {part.Number} has an invalid offset.");
            }

            if (part.Length < 1 || string.IsNullOrWhiteSpace(part.Sha256))
            {
                diagnostics.Add($"Part {part.Number} has invalid length or hash.");
            }

            expectedOffset += part.Length;
            expectedNumber++;
        }

        if (expectedOffset != plan.ContentLength)
        {
            diagnostics.Add("Part lengths do not equal the total content length.");
        }

        return new ObjectPlanValidation(diagnostics.Count == 0, diagnostics);
    }
}
