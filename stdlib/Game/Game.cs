namespace CarbyScript.stdlib.Game
{
    public class Game
    {
        private readonly Plugin plugin;
        public readonly Chat chat;

        public Game(Plugin plugin)
        {
            this.plugin = plugin;
            this.chat = new Chat(plugin);
        }
    }
}