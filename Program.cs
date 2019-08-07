using System;
using System.IO;
using DataStreamProcess.Processing.Units.CommentParsers;
using System.Linq;

namespace stage_parser
{
    public class Program
    {
        public static void Main()
        {
            int offerCounter = 0;
            int errorCounter = 0;
            using (OfferContext db = new OfferContext())
            {
                var offer = db.Offers.Where(x => x.raw_floor_level.HasValue /*|| x.Multilevel_floor.HasValue*/).ToList();
                foreach (var e in offer)
                {
                    offerCounter++;
                    var floor_level = new CommentStageParser(e.description);
                    int number;
                    if (Int32.TryParse(floor_level.GetParserResult(), out number))
                    {
                        if (e.raw_floor_level != number)
                            Console.WriteLine("тест не пройден\n{0}\nОжидался вывод: {1}, вместо {2}\n",
                                e.description, e.raw_floor_level, floor_level.GetParserResult(), errorCounter++);
                    }
                    else
                        Console.WriteLine("Парсер вернул не число");
                }
            }
            Console.WriteLine("\nВсего тестов: {0}", offerCounter);
            Console.WriteLine("Тестов не пройдено: {0}", errorCounter);
        }
    }
}