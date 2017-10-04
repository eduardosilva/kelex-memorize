using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kelex_memorize.Infraestructure.DataAccess;

namespace kelex_memorize.Commands
{
    public class DeleteItem : KelexCommand
    {
        private IEnumerable<IKelexCommandParameter> parameters;

        public DeleteItem()
        {
            parameters = new[]
            {
                new KelexCommandParameter { Key = Constants.QUESTION_ID, Description = "Item identification", Required = true, ErrorRequiredMessage = "Nothing specified." }
            };
        }

        public override string Code
        {
            get { return "del"; }
        }

        public override string Description
        {
            get { return "Delete an item"; }
        }

        public override IEnumerable<IKelexCommandParameter> Parameters
        {
            get { return parameters; }
        }

        protected override void Execute()
        {
            var parameterValue = parameters.FirstOrDefault(p => p.Key == Constants.QUESTION_ID)?.Value;
            var questionId = 0;

            if (!(int.TryParse(parameterValue, out questionId) && questionId > 0))
            {
                Console.WriteLine("parameter -i has an invalid value");
                return;
            }

            using (var context = new DataContext())
            {
                var question = context.QuestionsAndAnswers.Find(questionId);

                if (question == null)
                {
                    Console.WriteLine("Item has not found");
                    return;
                }

                context.QuestionsAndAnswers.Remove(question);
                context.SaveChanges();

                Console.WriteLine("Item has removed successfully!");
            }
        }
    }
}
