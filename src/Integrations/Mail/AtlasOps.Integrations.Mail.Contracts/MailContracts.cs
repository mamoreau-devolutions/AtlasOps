namespace AtlasOps.Integrations.Mail.Contracts;

public sealed record MailAddress(string Address, string DisplayName);

public sealed record MailAttachment(
    string FileName,
    string ContentType,
    byte[] Content,
    string Sha256);

public sealed record MailMessagePlan(
    MailAddress Sender,
    IReadOnlyList<MailAddress> To,
    IReadOnlyList<MailAddress> Cc,
    IReadOnlyList<MailAddress> Bcc,
    string Subject,
    string TextBody,
    string? HtmlBody,
    IReadOnlyList<MailAttachment> Attachments,
    IReadOnlyDictionary<string, string> Headers);

public sealed record MailPlanValidation(
    bool Valid,
    long TotalAttachmentBytes,
    IReadOnlyList<string> Diagnostics);
