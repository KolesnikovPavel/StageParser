using System;
using System.IO;
using DataStreamProcess.Processing.Units.CommentParsers;
using System.Linq;

namespace stage_parser
{
    public class Program
    {
        public static void FloorWriter (string line, string newCsvPath)
        {
            using (var sw = new StreamWriter(newCsvPath, true))
                sw.WriteLine(line);
        }

        public static void Main()
        {
            int offerCounter = 0;
            int errorCounter = 0;
            string newCsvPath = @"C:\Users\matrix\Desktop\processed_floor.csv";
            File.WriteAllText(newCsvPath, String.Empty);
            using (OfferContext db = new OfferContext())
            {
                var offer = db.Offers.Where(x => x.raw_floor_level.HasValue).ToList();
                foreach (var e in offer)
                {
                    //Console.WriteLine($"{e.raw_floor_level}");
                    //Console.Write($"{e.description}\n\n");
                    offerCounter++;
                    int floor_level = 2; //тут нужно передать description из БД в CommentStageParser и получить значение этажа
                    if (e.raw_floor_level != floor_level)
                        Console.WriteLine("тест не пройден!\n {0} \nОжидался вывод: {1}, вместо {2}\n", 
                            e.description, e.raw_floor_level, floor_level, errorCounter++);
                }
            }
            Console.WriteLine("\nВсего тестов: {0}", offerCounter);
            Console.WriteLine("Тестов не пройдено: {0}",errorCounter);
        }
    }
}