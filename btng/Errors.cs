using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace btng
{
    public static class Errors
    {
        static ConsoleColor bg, fg;

        public static void InvalidProject()
        {
            Console.WriteLine("This is not a project directory.");
            Console.WriteLine("Create a project with --project new [name]");
        }

        public static void SceneDoesNotExists()
        {
            Console.WriteLine("Scene by this name does not exists.");
        }

        public static void SceneAlreadyExists()
        {
            Console.WriteLine("Scene by this name already exists.");
        }

        public static void ProjectAlreadyExists()
        {
            Console.WriteLine("Project by this name already exists.");
        }
    }
}
