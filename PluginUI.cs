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
        private string loadScriptPath = string.Empty;
        
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
            {
                return;
            }

            ImGui.SetNextWindowSize(new Vector2(400, 400), ImGuiCond.Always);
            if (ImGui.Begin("CarbyScript", ref this.visible, ImGuiWindowFlags.None))
            {
                if (ImGui.Button("Open REPL Console"))
                {
                    this.repl.IsVisible = !this.repl.IsVisible;
                }
                
                ImGui.Text("Script path:");
                ImGui.SameLine();
                ImGui.InputText("##scriptPath", ref this.loadScriptPath, 512);
                ImGui.SameLine();
                if (ImGui.Button("Execute"))
                {
                    this.plugin.Engine.EvaluateDocument(this.plugin.LoadScript(this.loadScriptPath));
                }

                ImGui.End();
            }
        }
    }
}