using System;
using System.IO;
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
        public V8ScriptEngine Engine => engine;
        
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

            this.config = (Configuration) this.pluginInterface.GetPluginConfig() ?? new Configuration();
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

        public string LoadScript(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Failed to load script: '{path}' does not exist");
            }

            var properPath = string.Empty;
            if (File.GetAttributes(path).HasFlag(FileAttributes.Directory))
            {
                var packageJsonPath = Path.Combine(path, "package.json");

                if (File.Exists(packageJsonPath))
                {
                    properPath = Path.Combine(path, ReadPackageDescriptor(packageJsonPath));
                }
                else
                {
                    throw new FileNotFoundException(
                        $"Failed to load script: no package.json present in directory '{path}'");
                }
            }
            else
            {
                if (path.EndsWith("package.json"))
                {
                    properPath = Path.Combine(Directory.GetParent(path).FullName, ReadPackageDescriptor(path));
                }
                else if (path.EndsWith(".js"))
                {
                    properPath = path;
                }
                else
                {
                    throw new FileNotFoundException(
                        $"Failed to load script: '{path}' is not a valid plugin module or script");
                }
            }

            return properPath;
        }

        private string ReadPackageDescriptor(string path)
        {
            var desc = PackageDescriptor.Load(path);
            if (!desc.Type.ToLower().Equals("carbyscript"))
            {
                throw new Exception(
                    $"Failed to load script: '{path}' is not a valid CarbyScript module (package.json 'type' is not 'carbyscript')");
            }

            if (desc.Main.Equals(string.Empty))
            {
                throw new Exception($"Failed to load script: '{path}' has no defined 'main' entry in package.json");
            }

            return desc.Main;
        }

        private V8ScriptEngine CreateEngine()
        {
            var eng = new V8ScriptEngine();

            eng.AddHostObject("console", this.jsConsole);
            eng.AddHostObject("game", this.jsGame);

            eng.DocumentSettings.AccessFlags = DocumentAccessFlags.EnableFileLoading;
            
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
        [HelpMessage("Opens the CarbyScript main window.")]
        public void OpenMainWindow(string command, string args)
        {
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