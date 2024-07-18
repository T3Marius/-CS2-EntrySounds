using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API;
using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API.Core.Attributes.Registration;


namespace EntrySounds
{
    public class SoundsConfig : BasePluginConfig
    {
        [JsonPropertyName("Tag")] public string Tag { get; set; } = "[{green}eSounds{default}] ";
        public Dictionary<string, string> EntrySounds { get; set; } = new Dictionary<string, string>();
        [JsonPropertyName("EntryMessage")] public string EntryMessage { get; set; } = "{blue}{playername}{orange} joined the server!";
    }

    public class EntrySounds : BasePlugin, IPluginConfig<SoundsConfig>
    {
        public override string ModuleAuthor => "T3Marius";
        public override string ModuleName => "EntrySounds";
        public override string ModuleVersion => "0.0.1";
        public override string ModuleDescription => "EntrySounds with custom permissions";
        public SoundsConfig Config { get; set; } = new SoundsConfig();

        public void OnConfigParsed(SoundsConfig config)
        {
            config.Tag = StringExtensions.ReplaceColorTags(config.Tag);
            config.EntryMessage = StringExtensions.ReplaceColorTags(config.EntryMessage);
            Config = config;
        }

        private string ReplaceEntryMessage(string message, string playerName)
        {
            return message.Replace("{playername}", playerName);
        }

        [GameEventHandler]
        public HookResult OnPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
        {
            var playerName = @event.Userid.PlayerName;
            var steamId = @event.Userid.SteamID.ToString();

            if (Config.EntrySounds.ContainsKey(steamId))
            {
                string message = ReplaceEntryMessage(Config.EntryMessage, playerName);
                string fullMessage = Config.Tag + message;

                Server.PrintToChatAll(fullMessage);
            }


            foreach (var sound in Config.EntrySounds)
            {
                if (steamId == sound.Key)
                {
                    Utilities.GetPlayers().ForEach(player =>
                    {
                        if (player.IsValid)
                        {
                            player.ExecuteClientCommand($"play {sound.Value}");
                        }
                    });
                }
            }

            return HookResult.Continue;
        }
    }
}
