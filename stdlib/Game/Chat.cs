using Dalamud.Game.Chat;

namespace CarbyScript.stdlib.Game
{
    public class Chat
    {
        private readonly Plugin plugin;

        public Chat(Plugin plugin)
        {
            this.plugin = plugin;
        }

        public void print(string text)
        {
            plugin.GetInterface().Framework.Gui.Chat.Print(text);
        }
        
        public void printError(string text)
        {
            plugin.GetInterface().Framework.Gui.Chat.PrintError(text);
        }
    }
}