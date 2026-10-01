namespace AtlasOps.Scheduling;

using System.Security.Cryptography;
using System.Text;

using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;

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
        Calendar calendar = new();
        calendar.Events.Add(new CalendarEvent
        {
            Summary = title,
            DtStart = new CalDateTime(startsAt.UtcDateTime),
            DtEnd = new CalDateTime(startsAt.Add(duration).UtcDateTime),
            Uid = $"atlasops-{Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(title + startsAt.ToString("O")))).ToLowerInvariant()}",
        });
        CalendarSerializer serializer = new();
        return new(true, next, serializer.SerializeToString(calendar), []);
    }
}
