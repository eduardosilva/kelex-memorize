using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kelex_memorize.Infrastructure.DataAccess;

namespace kelex_memorize.Commands
{
    public class Search : KelexCommand
    {
        private IEnumerable<IKelexCommandParameter> parameters;

        public Search()
        {
            parameters = new[]
            {
                new KelexCommandParameter { Key = Constants.CRITERIA_KEY, Description = "Search criteria", Required = true, ErrorRequiredMessage = "Nothing specified." },
            };
        }

        public override string Code
        {
            get { return "search"; }
        }

        public override string Description
        {
            get { return "Search for questions, anwsers or decks"; }
        }

        public override IEnumerable<IKelexCommandParameter> Parameters
        {
            get { return parameters; }
        }

        protected override void Execute()
        {
            var criteria = parameters.FirstOrDefault(p => p.Key == Constants.CRITERIA_KEY).Value;

            using (var context = new DataContext())
            {
                var result = context.QuestionsAndAnswers.Where(q => q.Question.Contains(criteria)
                                                                 || q.Answer.Contains(criteria)
                                                                 || q.Deck.Contains(criteria))
                                                        .ToArray();

                if (!result.Any())
                {
                    Console.WriteLine(@"Nothing has found with '{0}' criteria", criteria);
                    return;
                }


                foreach (var question in result)
                {
                    Console.WriteLine("{0} - {1} - {2}", question.Id, question.Deck, question.Question.BreakInLines());

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Answer: {0}", question.Answer.BreakInLines());
                    Console.ResetColor();
                    Console.WriteLine("");
                    Console.WriteLine("");
                }

            }
        }
    }
}
