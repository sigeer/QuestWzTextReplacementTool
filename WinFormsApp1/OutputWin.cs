using Serilog;
using WeifenLuo.WinFormsUI.Docking;

namespace WinFormsApp1
{
    internal class OutputWin : DockContent
    {
        private RichTextBox richTextBox1;
        public OutputWin()
        {
            Text = "输出";
            richTextBox1 = new RichTextBox()
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
            };
            Controls.Add(richTextBox1);

            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.Sink(new WinFormsSink(AppendLog))
            .CreateLogger();
        }

        private void AppendLog(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(AppendLog), text);
                return;
            }

            richTextBox1.AppendText(text);
            richTextBox1.SelectionStart = richTextBox1.TextLength;
            richTextBox1.ScrollToCaret();
        }

    }
}
