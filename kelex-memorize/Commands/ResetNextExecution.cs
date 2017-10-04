using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kelex_memorize.Infraestructure.DataAccess;

namespace kelex_memorize.Commands
{
    public class ResetNextExecution : KelexCommand
    {
        private IEnumerable<IKelexCommandParameter> parameters;

        public ResetNextExecution()
        {
            parameters = new[]
            {
                new KelexCommandParameter { Key = Constants.QUESTION_ID, Description = "Item identification" }
            };
        }

        public override string Code
        {
            get { return "reset"; }
        }

        public override string Description
        {
            get { return "Reset a next execution for all or an especific item."; }
        }

        public override IEnumerable<IKelexCommandParameter> Parameters
        {
            get { return parameters; }
        }

        protected override void Execute()
        {
            var parameterValue = parameters.FirstOrDefault(p => p.Key == Constants.QUESTION_ID)?.Value;
            var questionId = 0;

            if (!String.IsNullOrEmpty(parameterValue) && !(int.TryParse(parameterValue, out questionId) && questionId > 0))
            {
                Console.WriteLine("parameter -i has an invalid value");
                return;
            }

            using (var context = new DataContext())
            {
                var questions = context.QuestionsAndAnswers.Where(q => q.NextExecution != null);

                if (questionId > 0)
                    questions = questions.Where(q => q.Id == questionId);


                foreach (var question in questions)
                {
                    question.NextExecution = null;
                }


                context.SaveChanges();

                Console.WriteLine("Next execution has reset successfully!");
            }
        }
    }
}
