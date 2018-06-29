using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kelex_memorize.Infrastructure.DataAccess;

namespace kelex_memorize.Commands
{
    public class List : KelexCommand
    {
        public override string Code
        {
            get { return "list"; }
        }

        public override string Description
        {
            get { return "List all questions"; }
        }

        public override IEnumerable<IKelexCommandParameter> Parameters
        {
            get { return new KelexCommandParameter[] { }; }
        }

        protected override void Execute()
        {
            using (var context = new DataContext())
            {
                var questions = context.QuestionsAndAnswers.ToList();

                if (!questions.Any())
                {
                    Console.WriteLine("No one questions to ask you. Please execute command kelex add to add a new question");
                    return;
                }

                questions.ForEach(q => 
                {
                    Console.WriteLine("{0} - {1} - {2}", q.Id, q.Deck, q.Question.BreakInLines());
                    Console.WriteLine("");
                });
            }
        }
    }
}
