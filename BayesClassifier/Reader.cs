using System.Collections.Generic;
using System.IO;

namespace BayesClassifier
{
    public static class Reader
    {
        public static string DirectoryPath { get => Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName; }
        public static Dictionary<string, Statistic> ReadData(int percentage = 100)
        {
            string notSpamDataPath = Path.Combine(DirectoryPath, @"Data\", "NotSpam.txt");
            string spamDataPath = Path.Combine(DirectoryPath, @"Data\", "Spam.txt");

            string[] spamWords = Helper.TransformDataToWords(File.ReadAllLines(spamDataPath), percentage);
            string[] notSpamWords = Helper.TransformDataToWords(File.ReadAllLines(notSpamDataPath), percentage);

            var dictionary = new Dictionary<string, Statistic>();

            foreach (var word in spamWords)
            {
                if (!dictionary.ContainsKey(word))
                {
                    dictionary.Add(word, new Statistic{ SpamCount = 1 });
                }
                else
                {
                    dictionary[word].SpamCount++;
                }
            }

            foreach (var word in notSpamWords)
            {
                if (!dictionary.ContainsKey(word))
                {
                    dictionary.Add(word, new Statistic { NotSpamCount = 1 });
                }
                else
                {
                    dictionary[word].NotSpamCount++;
                }
            }

            return dictionary;
        }

        public static string[] ReadSpam() => File.ReadAllLines(Path.Combine(DirectoryPath, @"Data\", "Spam.txt"));
        public static string[] ReadNotSpam() => File.ReadAllLines(Path.Combine(DirectoryPath, @"Data\", "NotSpam.txt"));

    }
}
