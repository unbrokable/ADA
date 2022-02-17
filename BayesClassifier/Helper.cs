using System;
using System.Linq;

namespace BayesClassifier
{
    static class Helper
    {
        public static string[] TransformDataToWords(string[] data, double percentage)
            => String.Join(" ; ", data.Take((int)((percentage / 100) * data.Length)))
            .Split(new char[] { ',', ' ', '.', ';', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(w => w.ToLower())
            .Where(w => w.Length > 2).ToArray();
    }
}
