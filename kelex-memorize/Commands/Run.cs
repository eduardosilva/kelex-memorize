using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kelex_memorize.Entities;
using kelex_memorize.Infraestructure.DataAccess;

namespace kelex_memorize.Commands
{
    public class Run : KelexCommand
    {
        private IEnumerable<IKelexCommandParameter> parameters;
        private IEnumerable<QuestionAndAnswer> cache;
        private Random random = new Random();
        private IEnumerable<LevelOption?> levels;

        public Run()
        {
            levels = new LevelOption?[]
            {
                new LevelOption { Level = 0, Description = "0 - Soon ( < 1m )", Minutes = 1 },
                new LevelOption { Level = 1, Description = "1 - Good ( < 10m )", Minutes = 10 },
                new LevelOption { Level = 2, Description = "2 - Easy ( 4d )", Minutes = new TimeSpan(days:4,hours: 0, minutes:0, seconds:0).TotalMinutes }
            };

            parameters = new[]
             {
                new KelexCommandParameter { Key = Constants.DECK_KEY, Description = "Deck identification" }
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
            var deck = parameters.FirstOrDefault(p => p.Key == Constants.DECK_KEY).Value;

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
                        System.Threading.Thread.Sleep(new TimeSpan(hours: 0, minutes: 0, seconds: 1));
                        totalSeconds--;
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                    }
                    while (totalSeconds > 0);

                    continue;
                }

                var question = questions.ElementAt(random.Next(maxValue: questions.Count()));

                Console.WriteLine("{0} - {1} - {2}", question.Id, question.Deck, question.Question.BreakInLines());
                Console.Read();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Answer: {0}", question.Answer.BreakInLines());

                Console.ResetColor();
                levels.ToList().ForEach(l => Console.WriteLine(l?.Description));

                question.NextExecution = GetNextExecution();

                Save(question);
            }
            while (true);
        }

        private IEnumerable<QuestionAndAnswer> GetQuestions(string deck)
        {
            if (cache.Any())
                return cache.Where(q => q.NextExecution == null || q.NextExecution < DateTime.Now);

            using (var context = new DataContext())
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
            using (var context = new DataContext())
            {
                context.QuestionsAndAnswers.Attach(question);
                context.Entry(question).State = System.Data.Entity.EntityState.Modified;

                context.SaveChanges();
            }
        }

        private DateTime GetNextExecution()
        {
            short validInformed = -1;
            LevelOption? selectedLevel = null;
            DateTime nextExecution;
            do
            {
                var line = Console.ReadLine();

                if (!short.TryParse(line, out validInformed) || (selectedLevel = levels.FirstOrDefault(v => v?.Level == validInformed)) == null)
                {
                    if (!String.IsNullOrEmpty(line))
                        Console.WriteLine("Level invalid try again!");
                }

            } while (selectedLevel == null);

            nextExecution = DateTime.Now.AddMinutes(selectedLevel.Value.Minutes);
            return nextExecution;
        }

        public struct LevelOption
        {
            public short Level { get; set; }
            public double Minutes { get; set; }
            public string Description { get; set; }
        }
    }
}