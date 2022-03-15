namespace btng_wpf
{
    public class ConfigurationModel
    {
        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Match with key in configuration.js")]
        public string title { get; set; }


        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Match with key in configuration.js")]
        public string entryPoint { get; set; }


        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Match with key in configuration.js")]
        public string theme { get; set; }


        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Match with key in configuration.js")]
        public bool log { get; set; }

        public ConfigurationModel(string title, string entryPoint, string theme, bool log)
        {
            this.title = title;
            this.entryPoint = entryPoint;
            this.theme = theme;
            this.log = log;
        }

        public static ConfigurationModel Parse(string file)
        {
            string raw = File.ReadAllText(file);
            string json = raw[raw.IndexOf("{")..(raw.IndexOf("}") + 1)];
            return JsonSerializer.Deserialize<ConfigurationModel>(json)!;
        }
    }
}
