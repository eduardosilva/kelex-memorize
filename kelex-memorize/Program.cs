using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kelex_memorize.Commands;
using kelex_memorize.Infraestructure.DataAccess;

namespace kelex_memorize
{
    class Program
    {
        static void Main(string[] args)
        {
            Database.SetInitializer<DataContext>(null);

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
