using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace WinFormsApp1
{
    public class WinFormsSink : ILogEventSink
    {
        private readonly Action<string> _write;

        public WinFormsSink(Action<string> write)
        {
            _write = write;
        }

        public void Emit(LogEvent logEvent)
        {
            var message = logEvent.RenderMessage();
            _write($"[{logEvent.Level}] {message}{Environment.NewLine}");
        }
    }
}
