using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kelex_memorize.Entities;
using kelex_memorize.Infrastructure.DataAccess;

namespace kelex_memorize.Commands
{
    public class AddItem : KelexCommand
    {
        private IEnumerable<IKelexCommandParameter> parameters;

        public AddItem()
        {
            parameters = new[]
            {
                new KelexCommandParameter { Key = Constants.QUESTION_KEY, Description = "The question.", Required = true, ErrorRequiredMessage = "Nothing specified." },
                new KelexCommandParameter { Key = Constants.ANSWSER_KEY, Description = "The answer.", Required = false },
                new KelexCommandParameter { Key = Constants.DECK_KEY, Description = "Deck.", Required = false }
            };
        }

        public override string Code { get { return "add"; } }
        public override string Description { get { return "Add a new item."; } }
        public override IEnumerable<IKelexCommandParameter> Parameters { get { return parameters; } }

        protected override void Execute()
        {
            using (var context = new DataContext())
            {
                var question = parameters.FirstOrDefault(p => p.Key == Constants.QUESTION_KEY).Value;
                var answer = parameters.FirstOrDefault(p => p.Key == Constants.ANSWSER_KEY)?.Value;
                var deck = parameters.FirstOrDefault(p => p.Key == Constants.DECK_KEY)?.Value;

                if (context.QuestionsAndAnswers.Any(q => q.Question == question))
                {
                    Console.WriteLine("Question has already added!");
                    return;
                }

                var formatedAnswer = answer;
				if (answer.Contains("\\n")){
                    formatedAnswer = String.Join("\n", answer.Split(new[] { "\\n" }, StringSplitOptions.RemoveEmptyEntries).Select(a => a.Trim()));
                }

                context.QuestionsAndAnswers.Add(new QuestionAndAnswer { Question = question, Answer = formatedAnswer, Deck = deck });
                context.SaveChanges();

                Console.WriteLine("Question has added successfully!");
            }
        }
    }
}
