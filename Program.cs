using System;
using DataStreamProcess.Processing.Units.CommentParsers;
using System.Linq;

namespace stage_parser
{
    public class Program
    {
        public static string EngLttrsRplsr(string sentence)
        {
            string[] engLetters = new string[] { "A", "a", "E", "e", "T", "Y", "y", "O", "o", "P", "p", "H", "K", "k", "X", "x", "C", "c", "B", "b", "N", "n", "M", "m" };
            string[] ruLetters = new string[] { "А", "а", "Е", "е", "Т", "У", "у", "О", "о", "Р", "р", "Н", "К", "к", "Х", "х", "С", "с", "В", "ь", "И", "п", "М", "м" };
            for (int i = 0; i < engLetters.Length; i++)
                if (sentence.Contains(engLetters[i]))
                    sentence = sentence.Replace(engLetters[i], ruLetters[i]);
            return sentence;
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
                var offer = db.Offers.Where(x => (x.raw_floor_level.HasValue || x.Multilevel_floor.HasValue) /*&& x.id == 1977*/).ToList();
                foreach (var e in offer)
                {
                    offerCounter++;
                    var floor_level = new CommentStageParser(EngLttrsRplsr(e.description));
                    int number;
                    if (e.raw_floor_level.HasValue)
                        singleFloorCounter++;
                    if (e.Multilevel_floor.HasValue)
                        multiFloorCounter++;
                    if (Int32.TryParse(floor_level.GetParserResult(), out number))
                    {
                        if ((e.floor_level != number)&& !e.Multilevel_floor.HasValue) 
                            Console.WriteLine("id: {0} было: {1} стало: {2} должно {3}",
                                e.id, e.floor_level, number, e.raw_floor_level, changedValues++);

                        //if (e.Multilevel_floor.HasValue)
                        //    Console.WriteLine("Тест не пройден. id <{2}>\n{0}\nПолучен этаж <{1}> для мультиэтажного помещения\n",
                        //        e.description, number, e.id, errorCounter++);

                        if (e.raw_floor_level != number && e.raw_floor_level.HasValue)
                            Console.WriteLine("тест не пройден. id <{3}>\n{0}\nожидался вывод: <{1}>, вместо <{2}>\n",
                                e.description, e.raw_floor_level, number, e.id, errorCounter++);
                    }
                    else
                        if (!e.Multilevel_floor.HasValue)
                        Console.WriteLine("Парсер вернул не число. id <{0}>\n{1}\n", e.id, e.description, errorCounter++, noValueCounter++);
                }
            }
            Console.WriteLine("\nВсего тестов: {0}", offerCounter);
            Console.WriteLine("\nЗначений изменилось: {0}", changedValues);
            Console.WriteLine("Тесты для одноэтажных помещений: {0}", singleFloorCounter);
            Console.WriteLine("Тесты для мультиэтажных помещений: {0}", multiFloorCounter);
            Console.WriteLine("Количество тестов без значения: {0}", noValueCounter);
            Console.WriteLine("Тестов не пройдено: {0}", errorCounter);
        }
    }
}