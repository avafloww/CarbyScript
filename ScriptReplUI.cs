using System;
using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;

namespace CarbyScript
{
    public class ScriptReplUI
    {
        private readonly Plugin plugin;
        
        private bool visible = false;
        private readonly List<string> logText = new List<string>();
        private readonly object renderLock = new object();
        private string inputText = string.Empty;

        public ScriptReplUI(Plugin plugin)
        {
            this.plugin = plugin;
        }

        public bool IsVisible
        {
            get => this.visible;
            set => this.visible = value;
        }

        public void LogLine(string line)
        {
            lock (this.renderLock)
            {
                logText.Add(line);
            }
        }
        
        public void Draw()
        {
            if (!IsVisible)
                return;
            
            ImGui.SetNextWindowSize(new Vector2(800, 400), ImGuiCond.Always);
            if (ImGui.Begin("CarbyScript REPL", ref this.visible, ImGuiWindowFlags.None))
            {
                ImGui.InputText("##replCommand", ref this.inputText, 512);
                ImGui.SameLine();
                if (ImGui.Button("Execute"))
                {
                    this.plugin.Execute(this.inputText);
                    this.inputText = string.Empty;
                }
                
                ImGui.BeginChild("scrolling", new Vector2(0, 0), false, ImGuiWindowFlags.HorizontalScrollbar);
                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 0));

                lock (this.renderLock)
                {
                    foreach (var line in this.logText)
                    {
                        ImGui.Text(line);
                    }
                }
                
                ImGui.PopStyleVar();

                if (ImGui.GetScrollY() >= ImGui.GetScrollMaxY())
                {
                    ImGui.SetScrollHereY(1.0f);
                }
                
                ImGui.EndChild();

                ImGui.End();
            }
        }
    }
}
