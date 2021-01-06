using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace btng
{
    static class Program
    {
        static void Main(string[] args)
        {
            //args = new string[] { "--scene", "new", "entry-scene" };

            if (args.Contains("--project"))
            {
                if (args.Contains("new"))
                {
                    ProjectNew(args);
                }
                else if (args.Contains("delete"))
                {
                    ProjectDelete(args);
                }
                else if (args.Contains("update"))
                {
                    ProjectUpdate(args);
                }
            }
            else if (args.Contains("--scene"))
            {
                if (args.Contains("new"))
                {
                    SceneNew(args);
                }
                else if (args.Contains("delete"))
                {
                    SceneDelete(args);
                }
                else
                {
                    SceneList(args);
                }
            }
        }

        private static void ProjectUpdate(string[] args)
        {
            ArgKeyValue keyValue = new ArgKeyValue()
            {
                ArgKey = "update",
                ArgValue = args.NextAfter("update"),
            };

            Assembly exec = Assembly.GetExecutingAssembly();

            ResourceLocal[] resourceNames = exec.GetManifestResourceNames().Where(x => !x.Contains("scenes") && !x.Contains("configuration.js")).Select(x => new ResourceLocal()
            {
                ResourceUrl = x,
                LocalUrl = x.ToLocalUrl(keyValue.ArgValue),
            }).ToArray();

            resourceNames.WriteFiles(exec, (string file) =>
            {
                Console.WriteLine($"Updated '{file}'");
            });
        }

        private static void SceneList(string[] args)
        {
            if (!File.Exists("configuration.js"))
            {
                Errors.InvalidProject();
                return;
            }

            string[] scenes = Directory.GetDirectories("scenes");

            if (scenes.Length == 0)
            {
                Console.WriteLine("This project does not have scenes.");
            }
            else
            {
                Console.WriteLine("This project has the following scenes:");
                foreach (string scene in scenes)
                {
                    Console.WriteLine(scene);
                }

            }
        }

        private static void SceneDelete(string[] args)
        {
            if (!File.Exists("configuration.js"))
            {
                Errors.InvalidProject();
                return;
            }

            ArgKeyValue keyValue = new ArgKeyValue()
            {
                ArgKey = "delete",
                ArgValue = args.NextAfter("delete"),
            };

            if (!Directory.Exists($"scenes/{keyValue.ArgValue}"))
            {
                Errors.SceneDoesNotExists();
                return;
            }

            Directory.Delete($"scenes/{keyValue.ArgValue}", true);

            Console.WriteLine($"Deleted '{keyValue.ArgValue}'");

            string[] scenes = Directory.GetDirectories("scenes");

            if (scenes.Length == 0)
            {
                Directory.Delete("scenes");
            }
        }

        private static void SceneNew(string[] args)
        {
            if (!File.Exists("configuration.js"))
            {
                Errors.InvalidProject();
                return;
            }

            ArgKeyValue keyValue = new ArgKeyValue()
            {
                ArgKey = "new",
                ArgValue = args.NextAfter("new"),
            };

            if (Directory.Exists($"scenes/{keyValue.ArgValue}"))
            {
                Errors.SceneAlreadyExists();
                return;
            }

            Assembly exec = Assembly.GetExecutingAssembly();

            ResourceLocal[] resourceNames = exec.GetManifestResourceNames().Where(x => x.Contains("__sceneName")).Select(x => new ResourceLocal()
            {
                ResourceUrl = x,
                LocalUrl = x.ToLocalUrl().Replace("__sceneName", keyValue.ArgValue),
            }).ToArray();

            resourceNames.WriteFiles(exec, (string file) =>
            {
                Console.WriteLine($"Generated '{file}'");
            });
        }

        static void ProjectNew(string[] args)
        {
            ArgKeyValue keyValue = new ArgKeyValue()
            {
                ArgKey = "new",
                ArgValue = args.NextAfter("new"),
            };

            if (Directory.Exists(keyValue.ArgValue))
            {
                Errors.ProjectAlreadyExists();
                return;
            }

            Assembly exec = Assembly.GetExecutingAssembly();

            ResourceLocal[] resourceNames = exec.GetManifestResourceNames().Where(x => !x.Contains("scenes")).Select(x => new ResourceLocal()
            {
                ResourceUrl = x,
                LocalUrl = x.ToLocalUrl(keyValue.ArgValue),
            }).ToArray();

            resourceNames.WriteFiles(exec, (string file) =>
            {
                Console.WriteLine($"Generated '{file}'");
            });

            Console.WriteLine("Don't forget to add your entry scene to 'configuration.js'");
        }

        static void ProjectDelete(string[] args)
        {
            ArgKeyValue keyValue = new ArgKeyValue()
            {
                ArgKey = "delete",
                ArgValue = args.NextAfter("delete"),
            };

            if (!Directory.Exists(keyValue.ArgValue))
            {
                Console.WriteLine("No such project exists.");
                return;
            }

            Directory.Delete(keyValue.ArgValue, true);

            Console.WriteLine($"Deleted '{keyValue.ArgValue}'");
        }
    }
}
