using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Compact;

namespace PlatformService;

public static class PlatformServiceLogging
{
    /// <summary>
    /// 建立輸出單行 JSON 的 logger,並掛上 "service" 欄位。
    /// output 為 null 時寫到 Console(stdout);測試可傳入自訂 TextWriter 以擷取輸出。
    /// </summary>
    public static Serilog.ILogger CreateLogger(string serviceName, TextWriter? output = null)
    {
        var formatter = new CompactJsonFormatter();

        var config = new LoggerConfiguration()
            .Enrich.WithProperty("service", serviceName);

        config = output is null
            ? config.WriteTo.Console(formatter)
            : config.WriteTo.Sink(new TextWriterSink(output, formatter));

        return config.CreateLogger();
    }

    private sealed class TextWriterSink : ILogEventSink
    {
        private readonly TextWriter _writer;
        private readonly ITextFormatter _formatter;

        public TextWriterSink(TextWriter writer, ITextFormatter formatter)
        {
            _writer = writer;
            _formatter = formatter;
        }

        public void Emit(LogEvent logEvent)
        {
            _formatter.Format(logEvent, _writer);
            _writer.Flush();
        }
    }
}
