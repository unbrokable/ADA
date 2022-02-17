namespace BayesClassifier
{
    public class Statistic
    {
        public int SpamCount { get; set; }

        public int NotSpamCount { get; set; }

        public double ProbabilitySpam {
            get => GetNormilizedPropobility(SpamCount);
        }

        public double ProbabilityNotSpam
        {
            get => GetNormilizedPropobility(NotSpamCount);
        }

        private double GetNormilizedPropobility (int count) => (count * (count / (SpamCount + NotSpamCount)) + 0.5) / (count + 1);
    }
}

