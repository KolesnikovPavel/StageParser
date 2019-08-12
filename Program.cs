using System;
using DataStreamProcess.Processing.Units.CommentParsers;
using System.Linq;

namespace stage_parser
{
    public class Program
    {
        public static string ConvertEnglishLetters(string sentence)
        {
            string[] engLetters = new string[] { "A", "a", "E", "e", "T", "Y", "y", "O", "o", "P", "p", "H", "K", "k", "X", "x", "C", "c", "B", "b", "N", "n", "M", "m" };
            string[] ruLetters = new string[] { "А", "а", "Е", "е", "Т", "У", "у", "О", "о", "Р", "р", "Н", "К", "к", "Х", "х", "С", "с", "В", "ь", "И", "п", "М", "м" };
            for (int i = 0; i < engLetters.Length; i++)
                if (sentence.Contains(engLetters[i]))
                    sentence = sentence.Replace(engLetters[i], ruLetters[i]);
            return sentence;
        }

        public static bool MoreThanOneFloor(stage_parser.Offer e)
        {
            return e.Multilevel_floor.HasValue && !e.raw_floor_level.HasValue ? true : false;
        }

        public static void DisplayTestResult (stage_parser.Offer e, int number)
        {
            Console.WriteLine("Тест не пройден. id: {0}\n{1}\n Ожидался этаж: {2}, вместо {3}\n",
                e.id, e.description, e.raw_floor_level, number);
        }

        public static void DisplayCounters(int offerCounter, int changedValues, int singleFloorCounter, int multiFloorCounter, int noValueCounter, int errorCounter)
        {
            Console.WriteLine("\nВсего тестов: {0}", offerCounter);
            Console.WriteLine("Значений изменилось: {0}", changedValues);
            Console.WriteLine("Тесты для одноэтажных помещений: {0}", singleFloorCounter);
            Console.WriteLine("Тесты для мультиэтажных помещений: {0}", multiFloorCounter);
            Console.WriteLine("Количество тестов без значения: {0}", noValueCounter);
            Console.WriteLine("Тестов не пройдено: {0}", errorCounter);
        }

        public static int DisplayHowParserResultChanged (stage_parser.Offer e, int number, int changedValues)
        {
            if (e.floor_level != number && number != e.raw_floor_level)
                Console.WriteLine("id: <{0}> было: {1} стало: {2} должно {3}",
                    e.id, e.floor_level, number, e.raw_floor_level, changedValues++);
            return changedValues;
        }


        public static void Main()
        {
            int offerCounter = 0;
            int singleFloorCounter = 0;
            int multiFloorCounter = 0;
            int errorCounter = 0;
            int noValueCounter = 0;
            int changedValues = 0;

            using (OfferContext db = new OfferContext())
            {
                Console.WriteLine("Включить тесты для мультиэтажных помещений? (да/нет)");
                string multilevel = Console.ReadLine();
                Console.WriteLine("Посмотреть как изменились значения? (да/нет)");
                string test = Console.ReadLine();
                Console.Clear();

                var offer = db.Offers.Where(x => (x.raw_floor_level.HasValue || x.Multilevel_floor.HasValue) /*&& x.id == 2410*/).ToList();
                foreach (var e in offer)
                {
                    var floor_level = new CommentStageParser(ConvertEnglishLetters(e.description));
                    if (Int32.TryParse(floor_level.GetParserResult(), out int number))
                    {
                        if (test == "да")
                        {
                            if (number < 163)
                                changedValues = DisplayHowParserResultChanged(e, number, changedValues);
                        }
                        else
                        {
                            if (MoreThanOneFloor(e))
                            {
                                if (multilevel == "да")
                                {
                                    DisplayTestResult(e, number);
                                    multiFloorCounter++;
                                    errorCounter++;
                                    offerCounter++;
                                }
                            }
                            else
                            {
                                offerCounter++;
                                singleFloorCounter++;
                                if (e.raw_floor_level != number)
                                {
                                    DisplayTestResult(e, number);
                                    errorCounter++;
                                }
                            }
                        }
                    }
                    else
                        if (test != "да")
                            Console.WriteLine("Парсер вернул не число. id <{0}>\n{1}\n",
                                e.id, e.description, errorCounter++, noValueCounter++, offerCounter++);
                }
            }
            DisplayCounters(offerCounter, changedValues, singleFloorCounter, multiFloorCounter, noValueCounter, errorCounter);
        }
    }
}