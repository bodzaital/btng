using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.Json;

namespace btng_wpf
{
    /// <summary>
    /// Interaction logic for ProjectConfiguration.xaml
    /// </summary>
    public partial class ProjectConfiguration : Window
    {
        string ConfjsLocation;

        public ProjectConfiguration(string confjsLocation)
        {
            InitializeComponent();
            ConfjsLocation = confjsLocation;

            string raw = File.ReadAllText(ConfjsLocation);
            string json = raw[raw.IndexOf("{")..(raw.IndexOf("}") + 1)];
            ConfigurationModel conf = JsonSerializer.Deserialize<ConfigurationModel>(json)!;

            TitleText.Text = conf.title;
            EntryPointText.Text = conf.entryPoint;
            ThemeText.Text = conf.theme;
            LogCheck.IsChecked = conf.log;
        }

        private void EntryPointButtonClick(object sender, RoutedEventArgs e)
        {
            string? entryPoint = OpenFolderDialog.Open("Select the folder of the entry point scene.");

            if (entryPoint is null) return;

            EntryPointText.Text = new DirectoryInfo(entryPoint).Name;
        }

        private void ThemeButtonClick(object sender, RoutedEventArgs e)
        {
            string? theme = OpenFolderDialog.Open("Select the folder of the theme.");

            if (theme is null) return;

            ThemeText.Text = new DirectoryInfo(theme).Name;
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            string title = TitleText.Text;
            string entryPoint = EntryPointText.Text;
            string theme = ThemeText.Text;
            bool log = (bool)LogCheck.IsChecked!;

            string json = JsonSerializer.Serialize(new ConfigurationModel(title, entryPoint, theme, log));

            StringBuilder sb = new();
            sb.AppendLine("// =======================");
            sb.AppendLine("// The configuration file.");
            sb.AppendLine("// =======================");
            sb.AppendLine("// entryPoint: the name of the scene (without extensions) that serves as the first page.");
            sb.AppendLine("// title: the title of the story (displayed in the title bar).");
            sb.AppendLine("// debugMode: display runtime exceptions (like CORS blocking, etc).");
            sb.AppendLine("// log: display messages in the browser console.");

            sb.AppendLine($"let conf = {json};");

            File.WriteAllText(ConfjsLocation, sb.ToString());

            Close();
        }
    }
}
