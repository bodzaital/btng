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

namespace btng_wpf
{
    /// <summary>
    /// Interaction logic for NewSceneDialog.xaml
    /// </summary>
    public partial class NewSceneDialog : Window
    {
        private string FullProjectDirectory;
        public string? SceneName, SceneTitle;

        public NewSceneDialog(string fullProjectDirectory)
        {
            InitializeComponent();

            FullProjectDirectory = fullProjectDirectory;
        }

        private void NewSceneButton(object sender, RoutedEventArgs e)
        {
            SceneName = SceneNameText.Text;
            SceneTitle = SceneTitleText.Text;

            if (ContainsIllegalCharacters(SceneName))
            {
                MessageBox.Show(
                    $"Name \"{SceneName}\" contains one or more illegal characters.",
                    "Cannot create new scene",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                SceneNameText.Text = "";
                return;
            }

            DialogResult = true;
            Close();
        }

        private bool ContainsIllegalCharacters(string projectFolder)
        {
            return projectFolder.IndexOfAny(Path.GetInvalidFileNameChars()) != -1;
        }
    }
}
