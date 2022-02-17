using System;
using System.Collections.Generic;
using System.Linq;

namespace BayesClassifier
{
    public static class Classifier
    {
        public static (double Spam, double NotSpam) Determine(Dictionary<string, Statistic> words, string data)
        {
            string[] datas = Helper.TransformDataToWords(new string[] { data }, 100);

            return (0.5 * CalculateOdds(datas, st => st.ProbabilitySpam), 0.5 * CalculateOdds(datas, st => st.ProbabilityNotSpam));

            double CalculateOdds(string[] dataWords, Func<Statistic, double> func)
                 => dataWords.Select(w => words.ContainsKey(w) ? func(words[w]) : 1).Aggregate((f, s) => f * s);
        }

        public static double CalculatePercentageRigthAnswers(int percentage)
        {
            if (percentage == 0)
            {
                return 0;
            }

            var words = Reader.ReadData(percentage);
            int coutRightAnswers = 0;

            string[] notSpams = Reader.ReadNotSpam();
            string[] spams = Reader.ReadSpam();

            foreach (var item in notSpams)
            {
                (double spam, double notSpam) = Determine(words, item);

                if (notSpam > spam)
                {
                    coutRightAnswers++;
                }
            }

            foreach (var item in spams)
            {
                (double spam, double notSpam) = Determine(words, item);

                if (notSpam < spam)
                {
                    coutRightAnswers++;
                }
            }

            return Math.Round(((double)coutRightAnswers / (spams.Length + notSpams.Length)) * 100, 2);
        }
    }
}
