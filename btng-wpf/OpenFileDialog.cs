using System.Windows.Forms;

namespace btng_wpf
{
    public static class OpenFileDialog
    {
        /// <summary>
        /// Opens a file browser dialog.
        /// </summary>
        /// <param name="caption">Text that is displayed in the caption of the dialog.</param>
        /// <returns>The <see cref="string"/> selected path,
        /// or <see cref="null"/> if the browsing is canceled.</returns>
        public static string? Open(string caption)
        {
            // Set up dialog.
            using System.Windows.Forms.OpenFileDialog d = new()
            {
                Title = caption,
            };

            // Open and check if it was canceled (not OK).
            if (d.ShowDialog() != DialogResult.OK) return null;

            // Return path.
            return d.FileName;
        }
    }
}
