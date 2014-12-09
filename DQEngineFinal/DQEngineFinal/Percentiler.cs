using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DQEngineFinal
{
    class Percentiler
    {


        public static double getMax(List<double> sequence)
        {
            return sequence.Max();

        }


        public static double getMin(List<double> sequence)
        {
            return sequence.Min();

        }

        public static double giveRelativeError(List<double> sequence, double value)
        {
            return ((value - sequence.Min()) / getDistance(sequence)) * 100;
        }


        public static double getDistance(List<double> sequence)
        {
            return Math.Abs(sequence.Max() - sequence.Min());
        }


        /*
         * This gives the average data in a vector
         */
        double giveAverageDistance(List<double> vector)
        {
            double dv = 0.0;
            foreach (double d in vector)
            {
                dv = d + dv;
            }

            return (dv / vector.Count);
        }


        /*
         * Returns the factor to which a an aaray has to be scalled to get the accurate value
         * for plotting the graphs
         */
        public static double scallingFactor(List<double> sequence)
        {
            double abs = getDistance(sequence);

            bool round = true;
            int multiplier = 0;

            while (round)
            {

                if (Math.Round(abs) == 0)
                {
                    abs = abs * 10;
                    ++multiplier;
                }
                else
                {
                    round = false;
                }

            }

            return Math.Pow(10, multiplier);
        }


        public static List<double> precentileForEach(List<double> sequence)
        {
            List<double> percentileOfEachVariable = new List<double>();

            foreach (double d in sequence)
            {
                percentileOfEachVariable.Add(percentile(sequence, d));
            }

            return percentileOfEachVariable;
        }


        /*
         * This method calculates the percentile for a sequence 
         * and the value you wish to calculate the percentile for
         */
        public static double percentile(List<double> sequence, double excelPercentile)
        {

            int n = findRankofElement(sequence, excelPercentile);

            return ((double)n * 100 / sequence.Count);

        }

        public static int findRankofElement(List<double> sequence, double excelPercentile)
        {
            int index = 0;
            sequence.Sort();

            while (index < sequence.Count)
            {
                if (sequence[index] == excelPercentile)
                {

                    return index;
                }
                else
                {
                    index++;
                }
            }

            return 0;
        }

    }
}
