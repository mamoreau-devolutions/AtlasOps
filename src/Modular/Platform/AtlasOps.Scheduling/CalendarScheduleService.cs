namespace AtlasOps.Scheduling;

using System.Globalization;
using System.Security.Cryptography;
using System.Text;

using Quartz;

public sealed record CalendarScheduleResult(bool Valid, DateTimeOffset? NextOccurrence, string CalendarText, IReadOnlyList<string> Diagnostics);

public sealed class CalendarScheduleService
{
    public CalendarScheduleResult Create(
        string cronExpression,
        string title,
        DateTimeOffset startsAt,
        TimeSpan duration)
    {
        if (!CronExpression.IsValidExpression(cronExpression))
        {
            return new(false, null, string.Empty, ["Cron expression is invalid."]);
        }

        CronExpression expression = new(cronExpression);
        DateTimeOffset? next = expression.GetNextValidTimeAfter(startsAt);
        string uid = $"atlasops-{Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(title + startsAt.ToString("O")))).ToLowerInvariant()}";
        CalendarContentWriter calendar = new();
        calendar.WriteProperty("BEGIN", "VCALENDAR");
        calendar.WriteProperty("PRODID", "-//AtlasOps//AtlasOps Scheduling//EN");
        calendar.WriteProperty("VERSION", "2.0");
        calendar.WriteProperty("BEGIN", "VEVENT");
        calendar.WriteProperty("DTEND", CalendarContentWriter.FormatUtc(startsAt.Add(duration)));
        calendar.WriteProperty("DTSTAMP", CalendarContentWriter.FormatUtc(DateTimeOffset.UtcNow));
        calendar.WriteProperty("DTSTART", CalendarContentWriter.FormatUtc(startsAt));
        calendar.WriteProperty("SEQUENCE", "0");
        calendar.WriteProperty("SUMMARY", CalendarContentWriter.EscapeText(title));
        calendar.WriteProperty("UID", CalendarContentWriter.EscapeText(uid));
        calendar.WriteProperty("END", "VEVENT");
        calendar.WriteProperty("END", "VCALENDAR");
        return new(true, next, calendar.ToString(), []);
    }
}

/// <summary>
/// Minimal RFC 5545 content-line writer: CRLF line endings, TEXT value escaping, and folding of
/// content lines longer than 75 octets without splitting UTF-8 sequences.
/// </summary>
internal sealed class CalendarContentWriter
{
    private const int MaximumLineOctets = 75;

    private readonly StringBuilder builder = new();

    public static string FormatUtc(DateTimeOffset value)
    {
        return value.UtcDateTime.ToString("yyyyMMdd'T'HHmmss'Z'", CultureInfo.InvariantCulture);
    }

    public static string EscapeText(string value)
    {
        StringBuilder escaped = new(value.Length);
        for (int index = 0; index < value.Length; index++)
        {
            char character = value[index];
            switch (character)
            {
                case '\\':
                    escaped.Append(@"\\");
                    break;
                case ';':
                    escaped.Append(@"\;");
                    break;
                case ',':
                    escaped.Append(@"\,");
                    break;
                case '\r' when index + 1 < value.Length && value[index + 1] == '\n':
                    break;
                case '\r':
                case '\n':
                    escaped.Append(@"\n");
                    break;
                default:
                    escaped.Append(character);
                    break;
            }
        }

        return escaped.ToString();
    }

    public void WriteProperty(string name, string value)
    {
        string line = string.Concat(name, ":", value);
        int octets = 0;
        int limit = MaximumLineOctets;
        for (int index = 0; index < line.Length; index++)
        {
            int length = char.IsHighSurrogate(line[index]) && index + 1 < line.Length ? 2 : 1;
            int characterOctets = Encoding.UTF8.GetByteCount(line.AsSpan(index, length));
            if (octets + characterOctets > limit)
            {
                this.builder.Append("\r\n ");
                octets = 0;
                limit = MaximumLineOctets - 1;
            }

            this.builder.Append(line, index, length);
            octets += characterOctets;
            index += length - 1;
        }

        this.builder.Append("\r\n");
    }

    public override string ToString()
    {
        return this.builder.ToString();
    }
}
