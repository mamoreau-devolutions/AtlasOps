namespace AtlasOps.Integrations.Mail.Core;

using System.Security.Cryptography;

using AtlasOps.Integrations.Mail.Contracts;

public sealed class MailPlanningService
{
    private static readonly IReadOnlySet<string> RestrictedHeaders =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Bcc",
            "Authorization",
            "Proxy-Authorization",
        };

    public MailPlanValidation Validate(
        MailMessagePlan plan,
        long maximumAttachmentBytes,
        int maximumRecipients)
    {
        List<string> diagnostics = [];

        ValidateAddress(plan.Sender, "Sender", diagnostics);
        MailAddress[] recipients = plan.To
            .Concat(plan.Cc)
            .Concat(plan.Bcc)
            .ToArray();
        foreach (MailAddress recipient in recipients)
        {
            ValidateAddress(recipient, "Recipient", diagnostics);
        }

        if (recipients.Length == 0)
        {
            diagnostics.Add("At least one recipient is required.");
        }
        else if (recipients.Length > maximumRecipients)
        {
            diagnostics.Add($"Recipient count exceeds the limit of {maximumRecipients}.");
        }

        if (string.IsNullOrWhiteSpace(plan.Subject))
        {
            diagnostics.Add("Subject is required.");
        }

        long attachmentBytes = plan.Attachments.Sum(static attachment => attachment.Content.LongLength);
        if (attachmentBytes > maximumAttachmentBytes)
        {
            diagnostics.Add("Attachment bytes exceed the configured limit.");
        }

        foreach (MailAttachment attachment in plan.Attachments)
        {
            string hash = Convert.ToHexString(SHA256.HashData(attachment.Content));
            if (!string.Equals(hash, attachment.Sha256, StringComparison.OrdinalIgnoreCase))
            {
                diagnostics.Add($"Attachment '{attachment.FileName}' has an invalid hash.");
            }

            if (Path.GetFileName(attachment.FileName) != attachment.FileName)
            {
                diagnostics.Add($"Attachment '{attachment.FileName}' has an invalid file name.");
            }
        }

        foreach (string header in plan.Headers.Keys)
        {
            if (RestrictedHeaders.Contains(header))
            {
                diagnostics.Add($"Header '{header}' is managed by the mail transport.");
            }
        }

        return new MailPlanValidation(diagnostics.Count == 0, attachmentBytes, diagnostics);
    }

    public IReadOnlyDictionary<string, string> RedactHeaders(IReadOnlyDictionary<string, string> headers)
    {
        return headers.ToDictionary(
            static pair => pair.Key,
            pair => IsSensitive(pair.Key) ? "[REDACTED]" : pair.Value,
            StringComparer.OrdinalIgnoreCase);
    }

    private static void ValidateAddress(
        MailAddress address,
        string role,
        List<string> diagnostics)
    {
        try
        {
            System.Net.Mail.MailAddress parsed = new(address.Address);
            if (!string.Equals(parsed.Address, address.Address, StringComparison.OrdinalIgnoreCase))
            {
                diagnostics.Add($"{role} address is not canonical.");
            }
        }
        catch (FormatException)
        {
            diagnostics.Add($"{role} address '{address.Address}' is invalid.");
        }
    }

    private static bool IsSensitive(string header)
    {
        return header.Contains("authorization", StringComparison.OrdinalIgnoreCase) ||
               header.Contains("token", StringComparison.OrdinalIgnoreCase) ||
               header.Contains("secret", StringComparison.OrdinalIgnoreCase);
    }
}
