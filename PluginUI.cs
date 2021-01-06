using System;
using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;

namespace CarbyScript
{
    public class PluginUI
    {
        private readonly Plugin plugin;
        private readonly ScriptReplUI repl;
        
        private bool visible = false;
        private readonly List<string> logText = new List<string>();
        private readonly object renderLock = new object();
        private string inputText = string.Empty;

        public PluginUI(Plugin plugin)
        {
            this.plugin = plugin;
            this.repl = new ScriptReplUI(plugin);
        }

        public ScriptReplUI Repl => this.repl;

        public bool IsVisible
        {
            get => this.visible;
            set => this.visible = value;
        }

        public void Draw()
        {
            Repl.Draw();
            
            if (!IsVisible)
                return;
            
            ImGui.SetNextWindowSize(new Vector2(400, 400), ImGuiCond.Always);
            if (ImGui.Begin("CarbyScript", ref this.visible, ImGuiWindowFlags.None))
            {
                if (ImGui.Button("Open REPL Console"))
                {
                    this.repl.IsVisible = !this.repl.IsVisible;
                }
                
                ImGui.End();
            }
        }
    }
}
