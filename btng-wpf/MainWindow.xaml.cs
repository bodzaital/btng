using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using HtmlAgilityPack;

namespace btng_wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string? ProjectFolder;
        private readonly HttpClient dl = new();

        private string StatusTitleResetText = "No project opened.";

        public MainWindow()
        {
            InitializeComponent();
            StateNoProjectOpened();
            StateNoSceneOpened();
        }

        private void StateNoProjectOpened()
        {
            CloseProjectButton.Visibility = Visibility.Collapsed;
            ConfigurationButton.Visibility = Visibility.Collapsed;
            SceneList.Visibility = Visibility.Collapsed;
            SceneButtonsContainer.Visibility = Visibility.Collapsed;

            CreateProjectButton.Visibility = Visibility.Visible;
            OpenProjectButton.Visibility = Visibility.Visible;

            GrayscaleStatusTitle(StatusTitleResetText);
        }

        private void StateProjectOpened()
        {
            CloseProjectButton.Visibility = Visibility.Visible;
            ConfigurationButton.Visibility = Visibility.Visible;
            SceneList.Visibility = Visibility.Visible;
            SceneButtonsContainer.Visibility = Visibility.Visible;

            CreateProjectButton.Visibility = Visibility.Collapsed;
            OpenProjectButton.Visibility = Visibility.Collapsed;
        }

        private void StateNoSceneOpened()
        {
            SceneNameLabel.Visibility = Visibility.Collapsed;
            LinksFromHereLabel.Visibility = Visibility.Collapsed;
            LinksFromHereContainer.Visibility = Visibility.Collapsed;

            SceneDetailsPlaceholder.Visibility = Visibility.Visible;
        }

        private void StateSceneOpened()
        {
            SceneNameLabel.Visibility = Visibility.Visible;
            LinksFromHereLabel.Visibility = Visibility.Visible;
            LinksFromHereContainer.Visibility = Visibility.Visible;

            SceneDetailsPlaceholder.Visibility = Visibility.Collapsed;
        }

        private void ColorStatusTitle(string text)
        {
            ProjectNameLabel.Content = text;
            ProjectNameLabel.Background = new SolidColorBrush(Color.FromArgb(128, 176, 0, 0));
        }

        private void GrayscaleStatusTitle(string? text = null)
        {
            ProjectNameLabel.Content = text;

            if (text is null)
            {
                ProjectNameLabel.Content = new DirectoryInfo(ProjectFolder!).Name;
            }
            
            ProjectNameLabel.Background = new SolidColorBrush(Color.FromArgb(255, 238, 238, 238));
        }

        private void NewSceneButtonClick(object sender, RoutedEventArgs e)
        {
            NewSceneDialog newSceneDialog = new(ProjectFolder!);
            bool dialogResult = (bool)newSceneDialog.ShowDialog()!;

            if (!dialogResult) return;

            Task.Run(() =>
            {
                Dispatcher.Invoke(() => ColorStatusTitle("Creating scene..."));

                string cssuri = "https://raw.githubusercontent.com/bodzaital/btif/master/scenes/__sceneName/__sceneName.css";
                string htmluri = "https://raw.githubusercontent.com/bodzaital/btif/master/scenes/__sceneName/__sceneName.html";
                string jsuri = "https://raw.githubusercontent.com/bodzaital/btif/master/scenes/__sceneName/__sceneName.js";

                string css = dl.GetStringAsync(cssuri).Result;
                string html = dl.GetStringAsync(htmluri).Result;
                string js = dl.GetStringAsync(jsuri).Result;

                html = html.Replace("__sceneName", newSceneDialog.SceneName);
                html = html.Replace("__sceneTitle", newSceneDialog.SceneTitle);

                css = css.Replace("__sceneName", newSceneDialog.SceneName);
                css = css.Replace("__sceneTitle", newSceneDialog.SceneTitle);

                js = js.Replace("__sceneName", newSceneDialog.SceneName);
                js = js.Replace("__sceneTitle", newSceneDialog.SceneTitle);

                Directory.CreateDirectory(Path.Combine(ProjectFolder!, "scenes", newSceneDialog.SceneName));

                File.WriteAllText(Path.Combine(ProjectFolder!, "scenes", newSceneDialog.SceneName, $"{newSceneDialog.SceneName}.html"), html);
                File.WriteAllText(Path.Combine(ProjectFolder!, "scenes", newSceneDialog.SceneName, $"{newSceneDialog.SceneName}.css"), css);
                File.WriteAllText(Path.Combine(ProjectFolder!, "scenes", newSceneDialog.SceneName, $"{newSceneDialog.SceneName}.js"), js);

                Dispatcher.Invoke(() =>
                {
                    GrayscaleStatusTitle();
                    UpdateSceneList();
                });
            });
        }

        private void CreateProjectClick(object sender, RoutedEventArgs e) => OpenOrCreateProject(true);

        private void LoadProjectClick(object sender, RoutedEventArgs e) => OpenOrCreateProject();

        private void OpenOrCreateProject(bool creatingNewProject = false)
        {
            OpenProjectDialog openProjectDialog = new(creatingNewProject);
            bool dialogResult = (bool)openProjectDialog.ShowDialog()!;

            if (!dialogResult) return;

            StateProjectOpened();

            ProjectFolder = openProjectDialog.ProjectFolder!;

            if (creatingNewProject)
            {
                List<(string relPath, string url)> projectTemplates = new()
                {
                    ("app.js",                    "https://raw.githubusercontent.com/bodzaital/btif/master/app.js"),
                    ("configuration.js",          "https://raw.githubusercontent.com/bodzaital/btif/master/configuration.js"),
                    ("index.html",                "https://raw.githubusercontent.com/bodzaital/btif/master/index.html"),
                    ("themes/default/frame.css",  "https://raw.githubusercontent.com/bodzaital/btif/master/themes/default/frame.css"),
                    ("themes/default/frame.html", "https://raw.githubusercontent.com/bodzaital/btif/master/themes/default/frame.html"),
                    ("themes/default/frame.js",   "https://raw.githubusercontent.com/bodzaital/btif/master/themes/default/frame.js"),
                    ("modules/utils.js",          "https://raw.githubusercontent.com/bodzaital/btif/master/modules/utils.js"),
                    ("modules/file.js",           "https://raw.githubusercontent.com/bodzaital/btif/master/modules/file.js"),
                    ("modules/globals.js",        "https://raw.githubusercontent.com/bodzaital/btif/master/modules/globals.js"),
                };

                Task.Run(() =>
                {
                    Dispatcher.Invoke(() => {
                        ColorStatusTitle("Creating project...");
                        StateNoProjectOpened();
                    });

                    Directory.CreateDirectory(Path.Combine(ProjectFolder, "themes/default"));
                    Directory.CreateDirectory(Path.Combine(ProjectFolder, "modules"));
                    Directory.CreateDirectory(Path.Combine(ProjectFolder, "scenes"));

                    foreach ((string relPath, string url) in projectTemplates)
                    {
                        File.WriteAllText(Path.Combine(ProjectFolder, relPath), dl.GetStringAsync(url).Result);
                    }

                    Dispatcher.Invoke(() => {
                        GrayscaleStatusTitle();
                        StateProjectOpened();
                        UpdateSceneList();
                    });
                });

            }
            else
            {
                // So the dispatcher above correctly runs at the end of the task.
                ProjectNameLabel.Content = new DirectoryInfo(ProjectFolder).Name;

                StateProjectOpened();
                UpdateSceneList();
            }
        }

        private void UpdateSceneList()
        {
            SceneListReplacement.Content = "No scenes in project.";
            SceneListReplacement.Visibility = Visibility.Visible;
            SceneList.Visibility = Visibility.Collapsed;

            if (ProjectFolder is null) return;

            // This is going to get ugly. Cover your eyes.
            string? selectedSceneName = ((ListBoxItem?)SceneList.SelectedItem)?.Content?.ToString();

            string[] scenes = Directory.GetDirectories(Path.Combine(ProjectFolder, "scenes"));
            if (scenes.Length == 0) return;

            SceneList.Items.Clear();
            foreach (string scene in scenes)
            {
                ListBoxItem l = new()
                {
                    Content = new DirectoryInfo(scene).Name,
                    Padding = new(0, 5, 0, 5),
                    Style = FindResource("SceneListItem") as Style,
                };

                SceneList.Items.Add(l);
            }

            SceneListReplacement.Visibility = Visibility.Collapsed;
            SceneList.Visibility = Visibility.Visible;

            if (selectedSceneName is null) return;

            foreach (ListBoxItem label in SceneList.Items)
            {
                if (label.Content.ToString() == selectedSceneName)
                {
                    SceneList.SelectedItem = label;
                }
            }

            //StateNoSceneOpened();
            HandleSceneSelected();
        }

        private void ProjectConfiguration(object sender, RoutedEventArgs e)
        {
            if (ProjectFolder is null) return;

            ProjectConfiguration pc = new(Path.Combine(ProjectFolder, "configuration.js"));
            bool pcResult = (bool)pc.ShowDialog()!;
        }

        private void OpenInVisualStudioCode(object sender, RoutedEventArgs e)
        {

        }

        private void RevealInExplorer(object sender, RoutedEventArgs e)
        {

        }

        private void CloseProjectClick(object sender, RoutedEventArgs e)
        {
            ProjectFolder = null;

            StateNoProjectOpened();
        }

        private void DeleteSceneClick(object sender, RoutedEventArgs e)
        {
            string? selectedSceneName = ((ListBoxItem?)SceneList.SelectedItem)?.Content?.ToString();

            if (selectedSceneName is null)
            {
                MessageBox.Show(
                    "No scene selected.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );

                return;
            }

            MessageBoxResult confirmDialog = MessageBox.Show(
                $"Are you sure you want to delete scene \"{selectedSceneName}\"?",
                "Delete scene",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (confirmDialog == MessageBoxResult.No) return;

            Directory.Delete(Path.Combine(ProjectFolder!, "scenes", selectedSceneName), true);

            UpdateSceneList();
        }

        private void RefreshSceneClick(object sender, RoutedEventArgs e)
        {
            UpdateSceneList();
        }

        private int? GetSceneLinks(string sceneName)
        {
            LinksFromHereContainer.Children.Clear();

            //string? sceneName = GetSelectedSceneName();

            //if (sceneName is null) return null;

            HtmlDocument sourceScene = new();
            sourceScene.LoadHtml(File.ReadAllText(Path.Combine(ProjectFolder!, "scenes", sceneName, $"{sceneName}.html")));

            List<(int number, string sourceInnerText, string targetSceneName)> linksFromHere = new();

            List<HtmlNode> transits = sourceScene.DocumentNode.Descendants().Where((e) => e.Name == "a").ToList();
            foreach (HtmlNode transit in transits)
            {
                if (transit.Attributes["data-link"]?.Value is not null) continue;

                linksFromHere.Add(new(transit.Line, transit.InnerText, transit.GetAttributeValue("href", null)));
            }

            foreach ((int number, string sourceInnerText, string targetSceneName) in linksFromHere)
            {
                StackPanel sp = new()
                {
                    Orientation = Orientation.Horizontal,
                };

                Label line = new()
                {
                    Content = $"Line {number}: ",
                    VerticalContentAlignment = VerticalAlignment.Center,
                };

                Label l1 = new()
                {
                    Content = $"\"{sourceInnerText}\"",
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Foreground = Brushes.Maroon,
                };

                TextBlock arrow = new()
                {
                    Text = "⟶",
                    FontSize = 16,
                    FontFamily = new("Segoe UI"),
                };

                Label l2 = new()
                {
                    Content = targetSceneName,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    FontSize = 14,
                    FontFamily = new("Monaco"),
                };

                sp.Children.Add(line);
                sp.Children.Add(l1);
                sp.Children.Add(arrow);
                sp.Children.Add(l2);

                LinksFromHereContainer.Children.Add(sp);
            }

            return linksFromHere.Count;
        }

        private string? GetSelectedSceneName()
        {
            return ((ListBoxItem?)SceneList.SelectedItem)?.Content?.ToString();
        }

        private void SceneSelected(object sender, SelectionChangedEventArgs e)
        {
            HandleSceneSelected();
        }

        private void HandleSceneSelected()
        {
            string? sceneName = GetSelectedSceneName();
            if(sceneName is null)
            {
                StateNoSceneOpened();
                return;
            }

            int? num = GetSceneLinks(sceneName);
            if (num is null) return;

            StateSceneOpened();

            SceneNameLabel.Content = GetSelectedSceneName();
            LinksFromHereLabel.Content = $"{num} transits from here";
        }
    }
}
