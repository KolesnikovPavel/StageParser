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


        public static bool DatabaseHasFilledValues(stage_parser.Offer offer)
        {
            return offer.raw_floor_level.HasValue || offer.Multilevel_floor.HasValue;
        }

        public static bool CheckMultilevel(stage_parser.Offer offer, string check)
        {
            if (MoreThanOneFloor(offer))
            {
                return check.ToLower() == "да";
            }
            return true;
        }

        public static bool CheckNoValue(stage_parser.Offer offer, string check)
        {
            return check.ToLower() == "да";
        }


        public static bool MoreThanOneFloor(stage_parser.Offer offer)
        {
            return offer.Multilevel_floor.HasValue && !offer.raw_floor_level.HasValue;
        }

        public static int? ReturnKnownValue(stage_parser.Offer offer)
        {
            if (MoreThanOneFloor(offer))
                return offer.Multilevel_floor;
            else
                return offer.raw_floor_level;

        }

        public static bool ParserResultNotCorrect(int? knownValue, int parser_floor_level)
        {
            return knownValue != parser_floor_level;
        }

        public static int DisplayAndCountTestFailure(stage_parser.Offer offer, int parser_floor_level, int errorCounter)
        {
            Console.WriteLine("Тест не пройден. id: {0}\n{1}\nОжидался этаж: {2}, вместо {3}\n",
                offer.id, offer.description, ReturnKnownValue(offer), parser_floor_level);
            return ++errorCounter;
        }

        public static int DisplayAndCountNoValueTestFailure(stage_parser.Offer offer, int errorCounter)
        {
            Console.WriteLine("Парсер вернул не число. id <{0}>\n{1}\n",
                offer.id, offer.description);
            return ++errorCounter;
        }


        public static void DisplayAllCounters(int offerCounter, int errorCounter)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nВсего тестов: " + offerCounter);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Тестов не пройдено: " + errorCounter);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        //public static int DisplayHowParserResultChanged(stage_parser.Offer offer, int parser_floor_level, int changedValues)
        //{
        //    if (offer.floor_level != parser_floor_level /*&& parser_floor_level != offer.raw_floor_level*/)
        //    {
        //        Console.WriteLine("id: <{0}> было: {1} стало: {2} должно: {3}",
        //            offer.id, offer.floor_level, parser_floor_level, offer.raw_floor_level);
        //        return ++changedValues;
        //    }
        //    else
        //        return changedValues + 0;
        //}

        public static void Main()
        {
            int offerCounter = 0;
            int errorCounter = 0;
            //int changedValues = 0;
            Console.WriteLine("Включить тесты для многоуровневых помещений? (да/нет)");
            string checkMultilevelUserResponse = Console.ReadLine();
            Console.WriteLine("Включить тесты в которых этаж не был найден? (да/нет)");
            string checkNoValueUserResponse = Console.ReadLine();
            Console.Clear();
            using (OfferContext db = new OfferContext())
            {
                var offers = db.Offers.Where(offer => DatabaseHasFilledValues(offer) /*&& offer.id == 87*/).ToList();
                foreach (var offer in offers)
                {
                    offerCounter++;
                    var commentStageParser = new CommentStageParser(ConvertEnglishLetters(offer.description));
                    if (Int32.TryParse(commentStageParser.GetParserResult(), out int parser_floor_level))
                    {
                        //changedValues = DisplayHowParserResultChanged(offer, parser_floor_level, changedValues);
                        if (CheckMultilevel(offer, checkMultilevelUserResponse))
                            if (ParserResultNotCorrect(ReturnKnownValue(offer), parser_floor_level))
                                errorCounter = DisplayAndCountTestFailure(offer, parser_floor_level, errorCounter);
                    }
                    else
                    {
                        if (CheckNoValue(offer, checkNoValueUserResponse))
                            errorCounter = DisplayAndCountNoValueTestFailure(offer, errorCounter);
                    }
                }
            }
            DisplayAllCounters(offerCounter, errorCounter);
            //Console.WriteLine("Значений изменилось: " + changedValues);
        }
    }
}