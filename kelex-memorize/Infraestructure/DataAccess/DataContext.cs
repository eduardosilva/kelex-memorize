using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kelex_memorize.Entities;

namespace kelex_memorize.Infraestructure.DataAccess
{
    public class DataContext : DbContext
    {
        public DataContext()
        {
            Database.Log = (s) => Debug.WriteLine(s);
            Database.Log = (s) => Console.WriteLine(s);
        }

        public DbSet<QuestionAndAnswer> QuestionsAndAnswers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var configurations = typeof(DataContext).Assembly.GetTypes()
                                                             .Where(t => t.IsAbstract == false &&
                                                                         t.BaseType != null &&
                                                                         t.BaseType.IsGenericType &&
                                                                         (t.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>) ||
                                                                          t.BaseType.GetGenericTypeDefinition() == typeof(ComplexTypeConfiguration<>)))
                                                             .ToArray();

            foreach (var configuration in configurations)
            {
                dynamic configurationTypeInstance = Activator.CreateInstance(configuration);
                modelBuilder.Configurations.Add(configurationTypeInstance);
            }
        }
    }
}
