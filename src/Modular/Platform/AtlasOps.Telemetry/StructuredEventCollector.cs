namespace AtlasOps.Telemetry;

using System.Globalization;

using Serilog;
using Serilog.Core;
using Serilog.Events;

public sealed record StructuredEvent(string Level, string Message, IReadOnlyDictionary<string, string> Properties);

public sealed class StructuredEventCollector : IDisposable
{
    private readonly List<StructuredEvent> events = [];
    private readonly Logger logger;
    private readonly CollectingSink sink;

    public StructuredEventCollector()
    {
        this.sink = new CollectingSink(this.events);
        this.logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Sink(this.sink)
            .CreateLogger();
    }

    public IReadOnlyList<StructuredEvent> Events => this.events;

    public void RecordOperation(string operation, string resourceType, TimeSpan duration, bool succeeded)
    {
        this.logger.Information(
            "Operation {Operation} for {ResourceType} completed in {DurationMs} ms with success {Succeeded}",
            operation,
            resourceType,
            duration.TotalMilliseconds,
            succeeded);
    }

    public void Dispose()
    {
        this.logger.Dispose();
    }

    private sealed class CollectingSink : ILogEventSink
    {
        private readonly List<StructuredEvent> events;

        public CollectingSink(List<StructuredEvent> events)
        {
            this.events = events;
        }

        public void Emit(LogEvent logEvent)
        {
            Dictionary<string, string> properties = logEvent.Properties.ToDictionary(
                static pair => pair.Key,
                static pair => pair.Value.ToString(),
                StringComparer.Ordinal);
            this.events.Add(new(
                logEvent.Level.ToString(),
                logEvent.RenderMessage(CultureInfo.InvariantCulture),
                properties));
        }
    }
}
