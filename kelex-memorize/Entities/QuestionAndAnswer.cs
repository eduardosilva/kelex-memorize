using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kelex_memorize.Entities
{
    public class QuestionAndAnswer
    {
        public QuestionAndAnswer()
        {
            Level = 1;
        }

        public int Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public DateTime? NextExecution { get; set; }
        public int Level { get; set; }
        public string Deck { get; set; }

        public void SetLevel(int level)
        {
            Level = level;
            NextExecution = DateTime.Now.AddMinutes(level);
        }
    }
}