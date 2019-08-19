using NUnit.Framework;
using DataStreamProcess.Processing.Units.CommentParsers;
using System;

namespace StageParser.UnitTests.Services
{
    [TestFixture]
    public class StageParserService
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

        [TestCase(8, "Сдам блок из 4 кабинетов 169.4 кв.м. (ориентир БТИ), 8/10 этаж, ремонт, лифт, охрана, телефон, интернет. Цена 400р/кв.м.")]
        [TestCase(7, "- Аренда офиса в БЦ Клевер-Парк\n- Метраж 50м2\n- Этаж 7\n- Офис с ремонтом\n-Без комиссий")]
        [TestCase(7, "Отличное помещение под офис. Помещение на 7этаже, окна во двор . На первом этаже атриум, охрана -24 часа. Офисное здание В+, возможна парковка на внутреннем дворе.")]
        [TestCase(6, "Прoдаетcя готовый бизнес - танцевальный pеcторан Aurum. - Нeжилoe пoмeщeниe paсположено на 6 - этaжe 9 этажногo здaния TЦ «Aлмаз»; -Пoмещeние состoит из: тpи банкетныx зaла, куxoннaя зoна, подсобное пoмeщение, хoлл и кабинeты; -Oбщая площaдь: 622 кв.м.; -Bысота пoтoлкoв: 2, 75 м. – 3, 10 м.; Цeнтральные: электpичеcтвo, отопление, водоснабжение, канализация, охрана, пожарная система, вентиляция.Танцевальный ресторан расположен в самом центре города на пересечении улиц ул.Куйбышева и ул.Екатерининская в ТЦ «Алмаз» . Торговый центр «Алмаз» – это современный многофункциональный комплекс торгового, административного и бытового назначения.Построен в марте 2005 года.Большая паковочная зона на 7, 8, 9 этажах на 500 машиномест.Трафик посещения клиентами Торгового центра по статистике составил около 10 000 посетителей в день.")]
        public void ParserTest(int floor_level, string descriptionToTest)
        {
            var commentStageParser = new CommentStageParser(ConvertEnglishLetters(descriptionToTest));
            if (Int32.TryParse(commentStageParser.GetParserResult(), out int parser_floor_level))
                Assert.AreEqual(floor_level, parser_floor_level);
        }
    }
}