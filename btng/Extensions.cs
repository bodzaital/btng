using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace btng
{
    public static class Extensions
    {
        static readonly string BTNG_CONTAINER = "btng.btif_source.";

        /// <summary>
        /// Returns the next value after a given item in an array. Expects both the previous and next values to exist.
        /// </summary>
        public static T NextAfter<T>(this T[] array, T previous)
        {
            return array[Array.IndexOf(array, previous) + 1];
        }

        /// <summary>
        /// Turns a dot-separated path to a local Url.
        /// </summary>
        public static string ToLocalUrl(this string dotseparated, string projectName = null)
        {
            StringBuilder dotPath = new StringBuilder();

            if (projectName != null)
            {
                dotPath.Append(projectName).Append('.');
            }

            dotPath.Append(dotseparated[BTNG_CONTAINER.Length..]);

            string sDotPath = dotPath.ToString();

            return new StringBuilder(sDotPath).Replace('.', '/', 0, sDotPath.LastIndexOf('.')).ToString();
        }

        public static void WriteFiles(this ResourceLocal[] resourceNames, Assembly exec, Action<string> callback)
        {
            foreach (ResourceLocal resource in resourceNames)
            {
                Stream resourceStream = exec.GetManifestResourceStream(resource.ResourceUrl);
                using StreamReader sr = new StreamReader(resourceStream);
                Directory.CreateDirectory(Path.GetDirectoryName(resource.LocalUrl));
                File.WriteAllText(resource.LocalUrl, sr.ReadToEnd());
                callback(resource.LocalUrl);
            }
        }
    }
}
