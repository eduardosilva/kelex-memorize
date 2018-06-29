using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kelex_memorize.Entities;
using System.Data.Entity.Infrastructure.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace kelex_memorize.Infrastructure.Configurations
{
    public class QuestionAndAnswerConfiguration : EntityTypeConfiguration<QuestionAndAnswer>
    {
        public QuestionAndAnswerConfiguration()
        {
            Property(t => t.Question)
                .IsRequired();
        }
    }
}
