using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using kelex_memorize.Commands;
using kelex_memorize.Infrastructure.DataAccess;

namespace kelex_memorize
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            var fileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            Console.WriteLine("Kelex Memorize v{0}", fileVersion.FileVersion);
            Console.ResetColor();

            Database.SetInitializer<DataContext>(new DropCreateDatabaseIfModelChanges<DataContext>());

            var commands = typeof(IKelexCommand).Assembly.GetTypes()
                                                         .Where(t => t.IsAbstract == false &&
                                                                     t.BaseType != null &&
                                                                     t.BaseType == typeof(KelexCommand))
                                                         .Select(t => (IKelexCommand)Activator.CreateInstance(t))
                                                         .ToArray();

            if (!args.Any())
            {
                Console.WriteLine("List of commands");
                Console.WriteLine("");
                foreach (var command in commands)
                {
                    Console.WriteLine("{0}: {1}", command.Code, command.Description);
                }
                return;
            }

            var localizedCommand = commands.FirstOrDefault(c => args.Contains(c.Code));
            if (localizedCommand == null)
            {
                Console.WriteLine("kelex: '{0}' is not a kelex command. See kelex -help", args.FirstOrDefault());
                return;
            }

            Console.WriteLine("");
            localizedCommand.Execute(args);
        }
    }
}
