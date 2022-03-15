namespace btng_wpf
{
    /// <summary>
    /// Interaction logic for OpenProjectDialog.xaml
    /// </summary>
    public partial class OpenProjectDialog : Window
    {
        public string? ProjectFolder = null;
        private readonly bool CreatingNewProject = false;

        private readonly string browseLabelReset = "Choose project to open.";

        public OpenProjectDialog(bool create = false)
        {
            InitializeComponent();
            CreatingNewProject = create;

            if (CreatingNewProject)
            {
                CreateOpenHeader.Content = "Create project";
                CreateOpenButton.Content = "Create";
                browseLabelReset = "Choose folder to save into.";
                BrowseLabel.Content = browseLabelReset;
                Title = "Create project";
            }
        }

        private void BrowseProjectButtonClick(object sender, RoutedEventArgs e)
        {
            Type dialogType = CreatingNewProject ? typeof(OpenFolderDialog) : typeof(OpenFileDialog);
            string caption = CreatingNewProject ? "Select a folder to create your project" : "Select configuration.js";

            string? folderOrConfPath = dialogType.GetMethod("Open")!.Invoke(null, new[] { caption }) as string;

            if (folderOrConfPath is null) return;

            ProjectFolder = Directory.GetParent(folderOrConfPath)!.FullName;
            string? name = Directory.GetParent(folderOrConfPath)?.Name;

            if (CreatingNewProject)
            {
                ProjectFolder = folderOrConfPath;
                name = new DirectoryInfo(ProjectFolder)?.Name;
            }

            if (name is null)
            {
                MessageBox.Show(
                    $"This folder cannot be selected as a project folder.\n\nFor example, the drive root cannot be a project folder.",
                    "Cannot create project",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                Reset();
                return;
            }

            BrowseLabel.Content = name;
            CreateOpenButton.IsEnabled = true;

            if (CreatingNewProject && IsNotEmptyFolder(ProjectFolder))
            {
                MessageBoxResult mb = MessageBox.Show(
                    $"Folder \"{name}\" is not empty.\n\nAre you sure you want to continue?",
                    "This folder is not empty",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (mb == MessageBoxResult.No) Reset();
            }
            else if (!CreatingNewProject && BadConfigurationFile(ProjectFolder))
            {
                MessageBox.Show(
                    $"The folder \"{name}\" is not a project.\n\nAre you sure you selected configuration.js?",
                    "This is not a project",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );

                Reset();
                return;
            }
        }

        private void CreateButtonClick(object sender, RoutedEventArgs e)
        {
            HandleClose();
        }

        private void HandleClose(bool forceClose = true)
        {
            DialogResult = ProjectFolder is not null;
            if (forceClose) Close();
        }

        private void Reset()
        {
            ProjectFolder = null;
            BrowseLabel.Content = browseLabelReset;
            CreateOpenButton.IsEnabled = false;
        }

        private static bool IsNotEmptyFolder(string folder)
        {
            return Directory.GetFiles(folder).Length > 0;
        }

        private static bool BadConfigurationFile(string folder)
        {
            try
            {
                ConfigurationModel conf = ConfigurationModel.Parse(Path.Combine(folder, "configuration.js"));
            }
            catch (Exception)
            {
                return true;
            }

            return false;
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            HandleClose(false);
        }
    }
}
