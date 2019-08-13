using System;
using DataStreamProcess.Processing.Units.CommentParsers;
using System.Linq;

namespace stage_parser
{
    public class Program
    {
        public static string ConvertEnglishLetters(string description)
        {
            string[] engLetters = new string[] { "A", "a", "E", "e", "T", "Y", "y", "O", "o", "P", "p", "H", "K", "k", "X", "x", "C", "c", "B", "b", "N", "n", "M", "m" };
            string[] ruLetters = new string[] { "А", "а", "Е", "е", "Т", "У", "у", "О", "о", "Р", "р", "Н", "К", "к", "Х", "х", "С", "с", "В", "ь", "И", "п", "М", "м" };
            for (int i = 0; i < engLetters.Length; i++)
                if (description.Contains(engLetters[i]))
                    description = description.Replace(engLetters[i], ruLetters[i]);
            return description;
        }

        public static bool MoreThanOneFloor(stage_parser.Offer offer)
        {
            return offer.Multilevel_floor.HasValue && !offer.raw_floor_level.HasValue ? true : false;
        }

        public static bool DatabaseHasFilledValues(stage_parser.Offer offer)
        {
            return offer.raw_floor_level.HasValue || offer.Multilevel_floor.HasValue ? true : false;
        }

        public static void DisplayTestResult (stage_parser.Offer offer, int parser_floor_level)
        {
            Console.WriteLine("Тест не пройден. id: {0}\n{1}\n Ожидался этаж: {2}, вместо {3}\n",
                offer.id, offer.description, offer.raw_floor_level, parser_floor_level);
        }
        
        public static void CompareAndDisplayResult(stage_parser.Offer offer, int parser_floor_level)
        {
            if (offer.raw_floor_level != parser_floor_level)
                DisplayTestResult(offer, parser_floor_level);
        }

        public static int DisplayHowParserResultChanged(stage_parser.Offer offer, int parser_floor_level, int changedValues)
        {
            if (offer.floor_level != parser_floor_level && parser_floor_level != offer.raw_floor_level && offer.Multilevel_floor != 0)
                Console.WriteLine("id: <{0}> было: {1} стало: {2} должно: {3}",
                    offer.id, offer.floor_level, parser_floor_level, offer.raw_floor_level, changedValues++);
            return changedValues;
        }

        public static void Main()
        {
            int offerCounter = 0;
            int changedValues = 0;
            string checkMultilevel = "нет";
            using (OfferContext db = new OfferContext())
            {
                var offers = db.Offers.Where(offer => /*(DatabaseHasFilledValues(offer)) &&*/ offer.id < 100).ToList();
                foreach (var offer in offers)
                {
                    offerCounter++;
                    var commentStageParser = new CommentStageParser(ConvertEnglishLetters(offer.description));
                    if (Int32.TryParse(commentStageParser.GetParserResult(), out int parser_floor_level))
                    {
                        //changedValues = DisplayHowParserResultChanged(offer, parser_floor_level, changedValues);
                        //if (MoreThanOneFloor(offer))
                        //{
                        //    if (checkMultilevel != "нет")
                        //        DisplayTestResult(offer, parser_floor_level);
                        //}
                        //else
                        //{
                        //    CompareAndDisplayResult(offer, parser_floor_level);
                        //}
                    }
                    //else
                    //    Console.WriteLine("Парсер вернул не число. id <{0}>\n{1}\n",
                    //        offer.id, offer.description);
                }
            }
            Console.WriteLine("\nВсего тестов " + offerCounter);
            Console.WriteLine("Тестов изменилось " + changedValues);
        }
    }
}