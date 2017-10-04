using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kelex_memorize.Entities
{
    public class QuestionAndAnswer
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public DateTime? NextExecution { get; set; }
        public string Deck { get; set; }
    }
}