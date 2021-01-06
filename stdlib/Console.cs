namespace CarbyScript.stdlib
{
    public class Console
    {
        private readonly Plugin plugin;

        public Console(Plugin plugin)
        {
            this.plugin = plugin;
        }

        public void log(string text)
        {
            this.plugin.GetUI().Repl.LogLine($"[info] {text}");
        }

        public void error(string text)
        {
            this.plugin.GetUI().Repl.LogLine($"[err] {text}");
        }
    }
}