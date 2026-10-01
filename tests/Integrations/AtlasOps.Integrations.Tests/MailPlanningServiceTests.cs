namespace AtlasOps.Integrations.Tests;

using System.Security.Cryptography;

using AtlasOps.Integrations.Mail.Contracts;
using AtlasOps.Integrations.Mail.Core;

[TestClass]
public sealed class MailPlanningServiceTests
{
    [TestMethod]
    public void Validate_ValidPlanAtRecipientAndAttachmentLimits_IsValid()
    {
        MailPlanningService service = new();
        MailAttachment attachment = Attachment("report.txt", [1, 2]);
        MailMessagePlan plan = Plan(
            [new MailAddress("one@example.com", "One")],
            "Subject",
            [attachment]);

        MailPlanValidation result = service.Validate(plan, 2, 1);

        Assert.IsTrue(result.Valid);
        Assert.AreEqual(2L, result.TotalAttachmentBytes);
        Assert.IsEmpty(result.Diagnostics);
    }

    [TestMethod]
    public void Validate_InvalidAddressesLimitsHashPathAndRestrictedHeader_ReturnDiagnostics()
    {
        MailPlanningService service = new();
        MailMessagePlan plan = new(
            new MailAddress("not-an-address", "Sender"),
            [new MailAddress("also-invalid", "Recipient")],
            [new MailAddress("second@example.com", "Second")],
            [],
            "",
            "body",
            null,
            [new MailAttachment("../secret.txt", "text/plain", [1, 2], "BAD")],
            new Dictionary<string, string> { ["Authorization"] = "secret" });

        MailPlanValidation result = service.Validate(plan, 1, 1);

        Assert.IsFalse(result.Valid);
        Assert.AreEqual(2L, result.TotalAttachmentBytes);
        Assert.Contains("Sender address 'not-an-address' is invalid.", result.Diagnostics);
        Assert.Contains("Recipient count exceeds the limit of 1.", result.Diagnostics);
        Assert.Contains("Subject is required.", result.Diagnostics);
        Assert.Contains("Attachment bytes exceed the configured limit.", result.Diagnostics);
        Assert.Contains("Attachment '../secret.txt' has an invalid hash.", result.Diagnostics);
        Assert.Contains("Attachment '../secret.txt' has an invalid file name.", result.Diagnostics);
        Assert.Contains("Header 'Authorization' is managed by the mail transport.", result.Diagnostics);
    }

    [TestMethod]
    public void Validate_NoRecipients_IsRejected()
    {
        MailPlanningService service = new();

        MailPlanValidation result = service.Validate(Plan([], "Subject", []), 10, 10);

        Assert.Contains("At least one recipient is required.", result.Diagnostics);
    }

    [TestMethod]
    public void RedactHeaders_RedactsSensitiveNamesCaseInsensitivelyAndPreservesSafeValues()
    {
        MailPlanningService service = new();
        Dictionary<string, string> headers = new(StringComparer.OrdinalIgnoreCase)
        {
            ["X-Api-Token"] = "token",
            ["Client-Secret"] = "secret",
            ["AUTHORIZATION"] = "authorization",
            ["X-Correlation"] = "safe",
        };

        IReadOnlyDictionary<string, string> result = service.RedactHeaders(headers);

        Assert.AreEqual("[REDACTED]", result["x-api-token"]);
        Assert.AreEqual("[REDACTED]", result["client-secret"]);
        Assert.AreEqual("[REDACTED]", result["authorization"]);
        Assert.AreEqual("safe", result["x-correlation"]);
    }

    private static MailMessagePlan Plan(
        IReadOnlyList<MailAddress> to,
        string subject,
        IReadOnlyList<MailAttachment> attachments)
    {
        return new MailMessagePlan(
            new MailAddress("sender@example.com", "Sender"),
            to,
            [],
            [],
            subject,
            "body",
            null,
            attachments,
            new Dictionary<string, string>());
    }

    private static MailAttachment Attachment(string name, byte[] content)
    {
        return new MailAttachment(
            name,
            "text/plain",
            content,
            Convert.ToHexString(SHA256.HashData(content)));
    }
}
