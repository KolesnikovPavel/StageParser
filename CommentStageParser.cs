using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DataStreamProcess.Processing.Units.CommentParsers
{
    public class CommentStageParser
    {
        public CommentStageParser(string sentence)
        {
            this._sentence = sentence;
            this._stagesCollection.Clear();
            this._distinctStagesCollection.Clear();
            this._sentenceCollectionStopWords.Clear();
            this._resarr.Clear();
            this._stagesMatchIndex.Clear();

            this.CleanAndSplitSentence(this._sentence);
            this.PerformAllTypeMatcher();
        }

        string _sentence;
        private readonly int _minWordSize = 1;
        readonly List<string> _resarr = new List<string>();
        readonly List<int> _stagesCollection = new List<int>();
        List<int> _distinctStagesCollection = new List<int>();

        readonly List<int> _stagesMatchIndex = new List<int>();

        readonly List<bool> _sentenceCollectionStopWords = new List<bool>();
        MatchCollection _sentenceCollection;

        public void CleanAndSplitSentence(string sentence)
        {
            this._resarr.Clear();
            sentence = this.PreProcessSentence(sentence);
            string[] arr = sentence.Trim().ToLower().Split(new char[] { '-', ' ', '_', ',', '.', '(', ')', ';', '"', ':', '!', '?', '%', '@', '"' });
            for (int i = 0; i <= arr.Length - 1; i++)
            {
                if (arr[i].Trim().Length != 0 && arr[i].Trim().Length >= this._minWordSize)
                {
                    this._resarr.Add(arr[i].ToLower());
                }
            }
        }

        public string GetParserResult()
        {
            if (this._stagesCollection.Count > 0)
            {
                int minId = this._stagesMatchIndex.IndexOf(this._stagesMatchIndex.Min());
                return this._stagesCollection[minId].ToString();
            }
            else
            {
                return "";
            }
        }

        public void PerformAllTypeMatcher()
        {
            this._stagesCollection.Clear();
            this._stagesMatchIndex.Clear();
            string comment = this._sentence;

            //last element problem
            comment = comment + " ";
            //delete garbage
            comment = Regex.Replace(comment, @"&#[\d]+;", " ");

            comment = Regex.Replace(comment, @"[а-я]{1}[.,]{0,}[А-Я]{1}", WordWordReplacer);


            //reaplce round brackets            
            comment = Regex.Replace(comment, @"[()]+", " ");
            comment = Regex.Replace(comment, @"[\s]{2,}", " ");

            //make strings sparse
            comment = Regex.Replace(comment, @"[\d]{1}[А-Яа-я]+", this.EvalMatchDigitLetter);
            comment = Regex.Replace(comment, @"[А-Яа-я][\d]", WordDigitReplacer);

            //split to sentences

            this._sentenceCollection = Regex.Matches(comment, @"([А-Яа-я]+)([а-яА-Я\d, +\-:;A-–Za-z()«»#&\/\n]+?)([,.;!?]+)");

            //            sentenceCollection = Regex.Matches(comment, @"([А-Я]+)([а-яА-Я\d, +\-:;A-–Za-z()«»#&\/\n]+)([.;!?]+)");
            this.FindSentenceStopWords();

            comment = comment.ToLower();

            //Исключить значения где несколько этажей
            MatchCollection firstFloor = Regex.Matches(comment, @"((первый)+|(первом)+|\s+1[ \s-ыйои–]*)\s*(этаж|этаже)[ \s!.,-;]+?");
            int test11 = 0;
            if (firstFloor.Count > 0)
                test11 = 1;
            MatchCollection secondFloor = Regex.Matches(comment, @"((второй)+|(втором)+|\s+2[ \s-ыйои–]*)\s*(этаж|этаже)[ \s!.,-;]+?");
            int test22 = 0;
            if (secondFloor.Count > 0)
                test22 = 1;
            MatchCollection thirdFloor = Regex.Matches(comment, @"((третий)+|(третьем)+|\s+3[ \s-ыйои–]*)\s*(этаж|этаже)[ \s!.,-;]+?");
            int test33 = 0;
            if (thirdFloor.Count > 0)
                test33 = 1;
            MatchCollection fourthFloor = Regex.Matches(comment, @"((четвертый)+|(четвертом)+|\s+4[ \s-ыйои–]*)\s*(этаж|этаже)[ \s!.,-;]+?");
            int test44 = 0;
            if (fourthFloor.Count > 0)
                test44 = 1;
            MatchCollection fifthFloor = Regex.Matches(comment, @"((пятый)+|(пятом)+|\s+5[ \s-ыйои–]*)\s*(этаж|этаже)[ \s!.,-;]+?");
            int test55 = 0;
            if (fifthFloor.Count > 0)
                test55 = 1;
            MatchCollection sixthFloor = Regex.Matches(comment, @"((шестой)+|(шестом)+|\s+6[ \s-ыйои–]*)\s*(этаж|этаже)[ \s!.,-;]+?");
            int test66 = 0;
            if (sixthFloor.Count > 0)
                test66 = 1;
            MatchCollection seventhFloor = Regex.Matches(comment, @"((седьмой)+|(седьмом)+|\s+7[ \s-ыйои–]*)\s*(этаж|этаже)[ \s!.,-;]+?");
            int test77 = 0;
            if (seventhFloor.Count > 0)
                test77 = 1;
            MatchCollection eighthFloor = Regex.Matches(comment, @"((восьмой)+|(восьмом)+|\s+8[ \s-ыйои–]*)\s*(этаж|этаже)[ \s!.,-;]+?");
            int test88 = 0;
            if (eighthFloor.Count > 0)
                test88 = 1;
            MatchCollection ninthFloor = Regex.Matches(comment, @"((девятый)+|(девятом)+|\s+9[ \s-ыйои–]*)\s*(этаж|этаже)[ \s!.,-;]+?");
            int test99 = 0;
            if (ninthFloor.Count > 0)
                test99 = 1;
            MatchCollection tenthFloor = Regex.Matches(comment, @"((десятый)+|(десятом)+|\s+10[ \s-ыйои–]*)\s*(этаж|этаже)[ \s!.,-;]+?");
            int test100 = 0;
            if (tenthFloor.Count > 0)
                test100 = 1;
            MatchCollection groundFloor = Regex.Matches(comment, @"((цокольный)+|(цокольном)+|\s+0[ \s-ыйои–]*)\s*(этаж|этаже)[ \s!.,-;]+?");
            int test101 = 0;
            if (groundFloor.Count > 0)
                test101 = 1;
            MatchCollection basementFloor = Regex.Matches(comment, @"((подвальный)+|(подвальном)+|\s+-1[ \s-ыйои–]*)\s*(этаж|этаже)[ \s!.,-;]+?");
            int test102 = 0;
            if (basementFloor.Count > 0)
                test102 = 1;
            MatchCollection mansardFloor = Regex.Matches(comment, @"((мансардный)+|(мансарда)+)\s*(этаж|этаже)[ \s!.,-;]+?");
            int test103 = 0;
            if (mansardFloor.Count > 0)
                test103 = 1;
            MatchCollection mezzanineFloor = Regex.Matches(comment, @"(антресоль)+\s*(этаж|этаже)[ \s!.,-;]+?");
            int test104 = 0;
            if (mezzanineFloor.Count > 0)
                test104 = 1;
            int bigTest = test11 + test22 + test33 + test44 + test55 + test66 + test77 + test88 + test99 + test100 + test101 + test102 + test103 + test104;
            if (bigTest > 1)
                Console.WriteLine(_sentence + "\n");

            MatchCollection test = Regex.Matches(comment, @"(продается|продам|предлагаем|предлагается)+[\s\dА-я-:]*( здание| комплекс)+[\s\dА-я-,]*");
            MatchCollection test1 = Regex.Matches(comment, @"(земельный)+[\s\dА-я-]*(участок\s)+[\s\dА-я-]*");
            MatchCollection test2 = Regex.Matches(comment, @"(двух\s*уровневое)+");
            MatchCollection test3 = Regex.Matches(comment, @"(\d+[ \s-и]*\d+)[ \s-ыйо–]*этаж");
            MatchCollection test4 = Regex.Matches(comment, @"автосалон");
            if (test.Count == 0 && test1.Count == 0 && test2.Count == 0 && test3.Count == 0 && test4.Count == 0)
            {
                //Console.WriteLine(_sentence + "\n");


                //на 3 этаже, на третьем этаже  
                MatchCollection st0 = Regex.Matches(comment, @"(на[ \s]|[ ]*)+([\d]+|[первом]{4,}|[втором]{4,}|[третьем]{4,}|[подземном]{4,}|[цокольном]{4,}|[четвертом]{4,}|[пятом]{4,}|[шестом]{4,}|[седьмом]{4,}|[восьмом]{4,}|[девятом]{4,}|[десятом]{4,})[ \s-мое–]+этаже[ \s!.,-;]+?");
                // на третьем, мансардном, этаже
                MatchCollection st00 = Regex.Matches(comment, @"(на[ \s]|[ ]*)+([\d]+?|[первом]{4,}?|[втором]{4,}?|[третьем]{4,}?|[подземном]{4,}?|[цокольном]{4,}?|[четвертом]{4,}?|[пятом]{4,}?|[шестом]{4,}?|[седьмом]{4,}?|[восьмом]{4,}?|[девятом]{4,}?|[десятом]{4,}?)[ \s-мое–,]+?([, ]+?[а-я]{4,}?[, ]+?)этаже[ \s!.,-;]+?");

                //1-ий этаж, 2-ой этаж
                MatchCollection st1 = Regex.Matches(comment, @"\s(\d{1,2})[ \s-ыйои–]+этаж[ \s-!\/.,;]+?");

                //этаж 1,  этаж первый
                //MatchCollection st2 = Regex.Matches(comment, @"этаж[ :\s]+([\d]{1,2}|[первый]{4,}|[второй]{4,}|[третий]{4,}|[подземный]{4,}|[цокольный]{4,}|[четвертый]{4,}|[пятый]{4,}|[шестой]{4,}|[седьмой]{4,}|[восьмой]{4,}|[девятый]{4,}|[десятый]{4,})[ :.!,\s;]+?");
                MatchCollection st2 = Regex.Matches(comment, @"этаж[ –:\-\s]+([\d]+[ \s-ыйои–]*|[первый]{4,}|[второй]{4,}|[третий]{4,}|[подземный]{4,}|[цокольный]{4,}|[четвертый]{4,}|[пятый]{4,}|[шестой]{4,}|[седьмой]{4,}|[восьмой]{4,}|[девятый]{4,}|[десятый]{4,})[ :.!,\s;]+?(?!м).");

                //этаж 1/9 (этаж 1)
                MatchCollection st3 = Regex.Matches(comment, @"этаж[ \s\-]+[\d](?=[\/][\d])");

                //Test
                //1/9 этаж
                MatchCollection stTest1 = Regex.Matches(comment, @"[\d][\/][\d]+[\s]этаж");
                //первый/9 этаж (возвращает 1)
                MatchCollection stTest2 = Regex.Matches(comment, @"([первый]{4,}|[второй]{4,}|[третий]{4,}|[подземный]{4,}|[цокольный]{4,}|[цокольное]{4,}|[четвертый]{4,}|[пятый]{4,}|[шестой]{4,}|[седьмой]{4,}|[восьмой]{4,}|[девятый]{4,}|[десятый]{4,})[\/][\d]+[\s]этаж");
                //Первом
                //MatchCollection stTest3 = Regex.Matches(comment, @"([первом]{4,}|[втором]{4,}|[третьем]{4,}|[подземном]{4,}|[цокольном]{4,}|[четвертом]{4,}|[пятом]{4,}|[шестом]{4,}|[седьмом]{4,}|[восьмом]{4,}|[девятом]{4,}|[десятом]{4,})[\W]+этаж");
                MatchCollection stTest4 = Regex.Matches(comment, @"этаж[\/]этажность[!;.,:\s]*([\d])*[\/][\d]*");
                //1 этаж 9-ти этажного
                MatchCollection stTest5 = Regex.Matches(comment, @"([\d]+[ \s-ыйои–]*)этаж[ \s-!.,;]+[\d]+[ \s-тиx]*этаж");
                //По документам цоколь
                //MatchCollection stTest6 = Regex.Matches(comment, @"по документам[ !;.,:\s]*([первый]{4,}|[второй]{4,}|[третий]{4,}|[подземный]{4,}|[цокольный]{4,}|[цокольное]{4,}|[четвертый]{4,}|[пятый]{4,}|[шестой]{4,}|[седьмой]{4,}|[восьмой]{4,}|[девятый]{4,}|[десятый]{4,})");

                //на 1/9 (возвращает на 1)
                MatchCollection st4 = Regex.Matches(comment, @"[\s]на[ \s]+[\d](?=[\/][\d])");
                //первый этаж, второй этаж
                MatchCollection st5 = Regex.Matches(comment, @"([первый]{4,}|[второй]{4,}|[третий]{4,}|[подземный]{4,}|[цокольный]{6,}|[цокольное]{6,}|[четвертый]{4,}|[пятый]{4,}|[шестой]{4,}|[седьмой]{4,}|[восьмой]{4,}|[девятый]{4,}|[десятый]{4,})[ .!,\s;]+?(этаж|этаже)[ !;.,:\s]+");
                // in instead of on
                MatchCollection st6 = Regex.Matches(comment, @"([в]+[ \s]|[ ]*)+([\d]+|[первом]{4,}|[втором]{4,}|[третьем]{4,}|[подземном]{4,}|[цокольном]{4,}|[четвертом]{4,}|[пятом]{4,}|[шестом]{4,}|[седьмом]{4,}|[восьмом]{4,}|[девятом]{4,}|[десятом]{4,})[ \s-мое–]+этаже[ \s!.,-;]+?");
                //stages range
                MatchCollection st7 = Regex.Matches(comment, @"[\d]+[, \d\-ми]+этажах");


                //втором, третьем этажах
                MatchCollection st8 = Regex.Matches(comment, @"([первомых]{4,}|[второмых]{4,}|[третьемых]{4,}|[подземном]{4,}|[цокольном]{4,}|[четвертом]{4,}|[пятом]{4,}|[шестом]{4,}|[седьмом]{4,}|[восьмом]{4,}|[девятом]{4,}|[десятом]{4,})[, и]+([первомых]{4,}|[второмых]{4,}|[третьемых]{4,}|[подземном]{4,}|[цокольном]{4,}|[четвертом]{4,}|[пятом]{4,}|[шестом]{4,}|[седьмом]{4,}|[восьмом]{4,}|[девятом]{4,}|[десятом]{4,})[, ]+этажах");
                //stages range
                MatchCollection st9 = Regex.Matches(comment, @"[\d]+[, \dи]+этажи");
                //первого этажа
                MatchCollection st10 = Regex.Matches(comment, @"([первого]{4,}|[второго]{4,}|[третьего]{4,}|[подземного]{8,}|[цокольного]{8,}|[цокольное]{4,}|[четвертого]{4,}|[пятого]{4,}|[шестого]{4,}|[седьмого]{4,}|[восьмого]{4,}|[девятого]{4,}|[десятого]{4,})[ .!,\s;]+?(этажа)[ !;.,:\s]+");
                //первом или втором этаже
                MatchCollection st11 = Regex.Matches(comment, @"([первом]{4,}|[втором]{4,}|[третьем]{4,}|[подземном]{4,}|[цокольном]{4,}|[четвертом]{4,}|[пятом]{4,}|[шестом]{4,}|[седьмом]{4,}|[восьмом]{4,}|[девятом]{4,}|[десятом]{4,})[, ли]+([первом]{4,}|[втором]{4,}|[третьем]{4,}|[подземном]{4,}|[цокольном]{4,}|[четвертом]{4,}|[пятом]{4,}|[шестом]{4,}|[седьмом]{4,}|[восьмом]{4,}|[девятом]{4,}|[десятом]{4,})[, ]+этаже");

                MatchCollection st12 = Regex.Matches(comment, @"[\d]+[, \d\-го]+этажа");

                MatchCollection st13 = Regex.Matches(comment, @"этажи[ :\s\-]+([\d][\- \s]+[\d]+)(?=([ .,;!?\sA-Za-zА-Яа-я]+?))");

                MatchCollection st14 = Regex.Matches(comment, @"на[ \s]+[\d]+[ этаже]+(?=[\/][\d]+[ этажного]+)");

                string elementsToFind = @"([первом]{4,}|[втором]{4,}|[третьем]{4,}|[подземном]{4,}|[цокольном]{4,}|[четвертом]{4,}|[пятом]{4,}|[шестом]{4,}|[седьмом]{4,}|[восьмом]{4,}|[девятом]{4,}|[десятом]{4,})";
                this.FindStringStage(st8, elementsToFind, this._digitsArr1, this._digitInt1);

                //string elementsToFind = @"([первом]{4,}|[втором]{4,}|[третьем]{4,}|[подземном]{4,}|[цокольном]{4,}|[четвертом]{4,}|[пятом]{4,}|[шестом]{4,}|[седьмом]{4,}|[восьмом]{4,}|[девятом]{4,}|[десятом]{4,})";
                this.FindStringStage(st11, elementsToFind, this._digitsArr1, this._digitInt1);


                //worse on 260 первого этажа ТЦ
                elementsToFind = @"([первого]{4,}|[второго]{4,}|[третьего]{4,}|[подземного]{4,}|[цокольного]{4,}|[цокольное]{4,}|[четвертого]{4,}|[пятого]{4,}|[шестого]{4,}|[седьмого]{4,}|[восьмого]{4,}|[девятого]{4,}|[десятого]{4,})";
                this.FindStringStage(st10, elementsToFind, this._digitsArr1, this._digitInt1);

                //Test
                elementsToFind = @"([первый]{4,}|[второй]{4,}|[третий]{4,}|[подземный]{4,}|[цокольный]{4,}|[цокольное]{4,}|[четвертый]{4,}|[пятый]{4,}|[шестой]{4,}|[седьмой]{4,}|[восьмой]{4,}|[девятый]{4,}|[десятый]{4,})";
                this.FindStringStage(stTest2, elementsToFind, this._digitsArr1, this._digitInt2);

                //elementsToFind = @"по документам[ !;.,:\s]*([первый]{4,}|[второй]{4,}|[третий]{4,}|[подземный]{4,}|[цокольный]{4,}|[цокольное]{4,}|[четвертый]{4,}|[пятый]{4,}|[шестой]{4,}|[седьмой]{4,}|[восьмой]{4,}|[девятый]{4,}|[десятый]{4,})";
                //this.FindStringStage(stTest6, elementsToFind, this._digitsArr1, this._digitInt2);

                //sentence parser
                //@"([А-Я]+)([а-яА-Я\d, +\-:;A-–Za-z()«»#&\/\n]+)([.;!?]+)"

                //only for st0 and st2 and last st5   
                this.FindAlphaNumericStages(st00, this._digitsArr1, this._digitInt1, 4, 1);
                this.FindAlphaNumericStages(st0, this._digitsArr1, this._digitInt1, 3, 1);
                this.FindAlphaNumericStages(st6, this._digitsArr1, this._digitInt1, 3, 1);
                this.FindAlphaNumericStages(st2, this._digitsArr2, this._digitInt2, 2, 1);
                this.FindAlphaNumericStages(st5, this._digitsArr2, this._digitInt2, 2, 0);

                this.FindNumericStages(st1);
                this.FindNumericStages(st3);
                this.FindNumericStages(stTest1);
                this.FindNumericStages(stTest4);
                this.FindNumericStages(stTest5);
                this.FindNumericStages(st4);
                this.FindNumericStages(st7);
                this.FindNumericStages(st9);
                this.FindNumericStages(st12);
                this.FindNumericStages(st13);
                this.FindNumericStages(st14);
                this._distinctStagesCollection = this._stagesCollection.Distinct().ToList();
            }
        }

        private void FindSentenceStopWords()
        {
            this._sentenceCollectionStopWords.Clear();
            foreach (Match wd in this._sentenceCollection)
            {
                bool contains = false;
                foreach (string sw in this._stopWords)
                {
                    if (wd.Value.ToLower().Contains(sw))
                    {
                        contains = true;
                        break;
                    }
                }
                this._sentenceCollectionStopWords.Add(contains);
            }
        }

        readonly string[] _stopWords = { "продуктовый", "магазин", "столовая", "отеля", "отель", "фитнес", "кафе", "ресторан", "банк", "салон", "связи", "галерея", "ресепшн" };
        readonly string[] _digitsArr1 = new string[] { "первом", "втором", "третьем", "подземном", "цокольном", "четвертом", "пятом", "шестом", "седьмом", "восьмом", "девятом", "десятом" };
        readonly int[] _digitInt1 = new int[] { 1, 2, 3, -1, 0, 4, 5, 6, 7, 8, 9, 10 };
        readonly string[] _digitsArr2 = new string[] { "первый", "второй", "третий", "подземный", "цокольный", "цокольное", "четвертый", "пятый", "шестой", "седьмой", "восьмой", "девятый", "десятый" };
        readonly int[] _digitInt2 = new int[] { 1, 2, 3, -1, 0, 0, 4, 5, 6, 7, 8, 9, 10 };

        public void FindStringStage(MatchCollection col, string elementsToFind, string[] digitsArr, int[] digitInt)
        {
            foreach (Match c in col)
            {
                MatchCollection findWordDigits = Regex.Matches(c.Value, elementsToFind);
                foreach (Match wd in findWordDigits)
                {
                    //try to convert to digit
                    int distance = -1;
                    int wordId = -1;
                    int minDistance = Int32.MaxValue;
                    int minId = -1;
                    foreach (string s in digitsArr)
                    {
                        wordId++;
                        distance = CommentParsers.LevenshteinDistanceHolder.LevenshteinDistance(s, wd.Value);
                        if ((distance / wd.Value.Length) * 100 <= this._allowableWordFuzzinessPercent)
                        {
                            if (minDistance > distance)
                            {
                                minDistance = distance;
                                minId = wordId;
                            }
                        }

                    }
                    if (minId != -1)
                    {
                        this._stagesCollection.Add(digitInt[minId]);
                        this._stagesMatchIndex.Add(c.Index);
                    }
                }
            }

        }


        readonly int _allowableWordFuzzinessPercent = 35;
        public void FindAlphaNumericStages(MatchCollection col, string[] digitsArr, int[] digitInt, int minArrSize, int arrPointer)
        {
            foreach (Match c in col)
            {
                MatchCollection findDigits = Regex.Matches(c.Value, @"[\d]+");
                if (findDigits.Count > 0)
                {
                    foreach (Match d in findDigits)
                    {
                        this._stagesCollection.Add(int.Parse(d.Value));
                        this._stagesMatchIndex.Add(c.Index);
                    }
                }
                else
                {
                    //treat string values
                    string val = Regex.Replace(c.Value.Trim(), @"[!;,:.()]+", " ");
                    val = Regex.Replace(val, @"[\s]{2,}", " ");
                    //string val = c.Value.Trim()
                    string[] arr = val.ToLower().Split(new char[] { '-', ' ', '_', ',', '.', '(', ')', ';', '"', ':', '!', '?', '%', '@', '"' });
                    if (arr.Length >= minArrSize)
                    {
                        string toMatch = arr[arrPointer];
                        if (toMatch.Length == 0) { continue; }
                        int distance = -1;
                        int wordId = -1;
                        int minDistance = Int32.MaxValue;
                        int minId = -1;
                        foreach (string s in digitsArr)
                        {
                            wordId++;
                            distance = CommentParsers.LevenshteinDistanceHolder.LevenshteinDistance(s, toMatch);
                            if ((distance / toMatch.Length) * 100 <= this._allowableWordFuzzinessPercent)
                            {
                                if (minDistance > distance)
                                {
                                    minDistance = distance;
                                    minId = wordId;
                                }
                            }

                        }
                        if (minId != -1)
                        {
                            this._stagesCollection.Add(digitInt[minId]);
                            this._stagesMatchIndex.Add(c.Index);
                        }
                    }

                }
            }
        }

        public void FindNumericStages(MatchCollection col)
        {
            foreach (Match c in col)
            {
                MatchCollection findDigits = Regex.Matches(c.Value, @"[\d]+");
                if (findDigits.Count > 0)
                {
                    foreach (Match d in findDigits)
                    {
                        this._stagesCollection.Add(int.Parse(d.Value));
                        this._stagesMatchIndex.Add(c.Index);
                    }
                }
            }
        }

        public string EvalMatchDigitLetter(Match m)
        {
            string matchedval = m.Value;
            return matchedval[0] + " " + matchedval.Substring(1);
        }

        public string PreProcessSentence(string sentence)
        {
            //make digits sparse
            sentence = Regex.Replace(sentence, @"[\d]{1}[А-Яа-я]+", this.EvalMatchDigitLetter);
            sentence = Regex.Replace(sentence, @"[А-Яа-я][\d]", WordDigitReplacer);
            //may be replace FPD with TWOD ONED (more general digit)
            string w = Regex.Replace(sentence, @"[\d]{1,}[.,][\d]{1,}", "FPD");//remove all digits  
            return Regex.Replace(w, @"[\d]{1,}", this.DigitCodeMerger);//remove all digits  
        }

        public static string WordDigitReplacer(Match m)
        {
            string matchedval = m.Value;
            return matchedval[0] + " " + matchedval[1];
        }
        public static string WordWordReplacer(Match m)
        {
            string matchedval = m.Value;
            return matchedval.Substring(0, matchedval.Length - 1) + " " + matchedval[matchedval.Length - 1];
        }

        public string DigitCodeMerger(Match m)
        {
            string matchedval = m.Value;

            switch (matchedval.Length)
            {
                case 0:
                    return matchedval;
                case 1:
                    return "ONED";
                case 2:
                    return "TWOD";
                case 3:
                    return "THRD";
                case 4:
                    return "FOURD";
                default:
                    return "MULTYD";
            }
        }
    }
}