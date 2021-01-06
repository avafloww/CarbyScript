using System;
using CarbyScript.Attributes;
using CarbyScript.stdlib;
using CarbyScript.stdlib.Game;
using Dalamud.Game.ClientState.Structs.JobGauge;
using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;
using Console = CarbyScript.stdlib.Console;

namespace CarbyScript
{
    public class Plugin : IDalamudPlugin
    {
        private DalamudPluginInterface pluginInterface;
        private PluginCommandManager<Plugin> commandManager;
        private Configuration config;
        private PluginUI ui;
        private V8ScriptEngine engine;

        private Console jsConsole;
        private Game jsGame;
        
        public string Name => "CarbyScript";

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;

            this.config = (Configuration)this.pluginInterface.GetPluginConfig() ?? new Configuration();
            this.config.Initialize(this.pluginInterface);

            this.ui = new PluginUI(this);
            this.pluginInterface.UiBuilder.OnBuildUi += this.ui.Draw;

            this.commandManager = new PluginCommandManager<Plugin>(this, this.pluginInterface);

            this.jsConsole = new Console(this);
            this.jsGame = new Game(this);
            this.engine = CreateEngine();
        }

        public DalamudPluginInterface GetInterface()
        {
            return pluginInterface;
        }
        
        public PluginUI GetUI()
        {
            return ui;
        }
        
        private V8ScriptEngine CreateEngine()
        {
            var eng = new V8ScriptEngine();
            
            eng.AddHostObject("console", this.jsConsole);
            eng.AddHostObject("game", this.jsGame);
            
            return eng;
        }

        public void Execute(string code)
        {
            try
            {
                this.engine.Execute(code);
            }
            catch (ScriptEngineException ex)
            {
                foreach (var line in ex.ToString().Split('\n'))
                {
                    this.jsConsole.error(line);
                }
            }
        }
        
        [Command("/cscript")]
        [HelpMessage("Opens the CarbyScript window.")]
        public void OpenMainWindow(string command, string args)
        {
            // You may want to assign these references to private variables for convenience.
            // Keep in mind that the local player does not exist until after logging in.
            // var chat = this.pluginInterface.Framework.Gui.Chat;
            // var world = this.pluginInterface.ClientState.LocalPlayer.CurrentWorld.GameData;
            // this.engine.Execute(args);
            this.ui.IsVisible = !this.ui.IsVisible;
        }

        #region IDisposable Support
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            this.commandManager.Dispose();

            this.pluginInterface.SavePluginConfig(this.config);

            this.pluginInterface.UiBuilder.OnBuildUi -= this.ui.Draw;

            this.pluginInterface.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
