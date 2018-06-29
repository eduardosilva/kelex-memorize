using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kelex_memorize.Entities;
using kelex_memorize.Infrastructure.DataAccess;

namespace kelex_memorize.Commands
{
    public class Run : KelexCommand
    {
        private IEnumerable<IKelexCommandParameter> parameters;
        private IEnumerable<QuestionAndAnswer> cache;
        private Random random = new Random();
        private IEnumerable<LevelOption> levels;
        private DateTime? endTime;

        private string deck;
        private bool verbose;

        public Run()
        {


            levels = new LevelOption[]
            {
                new LevelOption { Level = 0, Description = "0 - Soon" },
                new LevelOption { Level = 1, Description = "1 - Good" },
                new LevelOption { Level = 2, Description = "2 - Easy" }
            };

            parameters = new[]
             {
                new KelexCommandParameter { Key = Constants.DECK_KEY, Description = "Deck identification" },
                new KelexCommandParameter { Key = Constants.VERBOSE_KEY, Description = "Verbose", NotRequiredAValue = true },
                new KelexCommandParameter { Key = Constants.TIMER_KEY, Description = "With a Timer (default 5 minutes)", NotRequiredAValue = true },
            };

            cache = new QuestionAndAnswer[] { };
        }

        public override string Code
        {
            get { return "run"; }
        }

        public override string Description
        {
            get { return "Run memorize"; }
        }

        public override IEnumerable<IKelexCommandParameter> Parameters
        {
            get { return parameters; }
        }

        protected override void Execute()
        {
            var questions = new QuestionAndAnswer[] { };
            deck = parameters.FirstOrDefault(p => p.Key == Constants.DECK_KEY).Value;
            verbose = parameters.Any(p => p.Key == Constants.VERBOSE_KEY && p.Declared);
            var timerParameter = parameters.FirstOrDefault(p => p.Key == Constants.TIMER_KEY && p.Declared);

            if (timerParameter != null)
            {
                double timerValue = 5;
                if (!String.IsNullOrEmpty(timerParameter.Value) && !Double.TryParse(timerParameter.Value, out timerValue))
                {
                    Console.WriteLine("parameter -t has an invalid value");
                    return;
                }

                endTime = DateTime.Now.AddMinutes(timerValue);
            }

            do
            {
                questions = GetQuestions(deck).ToArray();

                if (!questions.Any())
                {
                    Console.WriteLine("Congratulations! You have finished {0} for now.", String.IsNullOrWhiteSpace(deck) ? "all" : "this deck");
                    var totalSeconds = 60;


                    do
                    {
                        Console.WriteLine("Wating for: {0}", totalSeconds);
                        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
                        totalSeconds--;
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                    }
                    while (totalSeconds > 0);

                    continue;
                }

                var question = questions.ElementAt(random.Next(maxValue: questions.Count()));

                if (!String.IsNullOrWhiteSpace(question.Deck))
                    Console.WriteLine("{0} - {1} - {2}", question.Id, question.Deck, question.Question.BreakInLines());
                else
                    Console.WriteLine("{0} - {1}", question.Id, question.Question.BreakInLines());

                Console.Read();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Answer: {0}", question.Answer.BreakInLines());

                Console.ResetColor();

                CalcAndShowLevelsToAnswer(question);
                GetNextExecution(question);
                Save(question);

                if (endTime.HasValue && DateTime.Now > endTime)
                {
                    Console.WriteLine("Time's up!!");
                    return;
                }

                Console.Clear();
            }
            while (true);
        }

        private void CalcAndShowLevelsToAnswer(QuestionAndAnswer question)
        {
            levels.ElementAt(0).Minutes = question.Level;
            for (int i = 1; i < levels.Count(); i++)
            {
                levels.ElementAt(i).Minutes = Math.Ceiling(((levels.ElementAt(i - 1).Minutes * 50) / 100) + levels.ElementAt(i - 1).Minutes);
            }

            Console.WriteLine("");
            levels.ToList().ForEach(l => Console.WriteLine("{0} - ( {1} min. )", l.Description, l.Minutes));
        }

        private IEnumerable<QuestionAndAnswer> GetQuestions(string deck)
        {
            if (cache.Any())
                return cache.Where(q => q.NextExecution == null || q.NextExecution < DateTime.Now);

            using (var context = new DataContext(verbose))
            {
                var query = context.QuestionsAndAnswers.AsQueryable();

                if (!String.IsNullOrWhiteSpace(deck))
                    query = query.Where(q => q.Deck == deck);

                cache = query.Where(q => q.NextExecution == null || q.NextExecution < DateTime.Now).ToArray();
            }

            return cache;
        }

        private void Save(QuestionAndAnswer question)
        {
            using (var context = new DataContext(verbose))
            {
                context.QuestionsAndAnswers.Attach(question);
                context.Entry(question).State = System.Data.Entity.EntityState.Modified;

                context.SaveChanges();
            }
        }

        private void GetNextExecution(QuestionAndAnswer question)
        {
            short validInformed = -1;
            LevelOption selectedLevel = null;
            do
            {
                var line = Console.ReadLine();

                if (!short.TryParse(line, out validInformed) || (selectedLevel = levels.FirstOrDefault(v => v?.Level == validInformed)) == null)
                {
                    if (!String.IsNullOrEmpty(line))
                        Console.WriteLine("Level invalid try again!");
                }

            } while (selectedLevel == null);

            question.SetLevel((int)selectedLevel.Minutes);

        }
    }
}