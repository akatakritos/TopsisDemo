namespace TopsisDemo;

// https://medium.com/analytics-vidhya/how-to-decide-between-multiple-equally-good-things-simple-use-math-7e517f1422d5

public static class Topsis
{
    struct ValuesIdeal
    {
        public double Worst { get; }
        public double Best { get; }
        public double[] CriterionValues { get; }

        public ValuesIdeal(double idealWorst, double idealBest, double[] criterionValues)
        {
            Worst = idealWorst;
            Best = idealBest;
            CriterionValues = criterionValues;
        }
    }


    public static TopsisResult<T>[] Compute<T>(IReadOnlyList<Criteria<T>> criteria, IReadOnlyList<T> items)
    {
        var sumWeight = criteria.Sum(s => s.Weight);
        var ideals = new ValuesIdeal[criteria.Count];

        for (var c = 0; c < criteria.Count; c++)
        {
            var criterion = criteria[c];

            // a column in most explanations: represents the value of this criterion for each item
            var criterionValues = new double[items.Count];

            // going to normalize all values by dividing by the square root of the sum of squares. We'll calculate
            // the sqrt later so we don't have to pass through the array multiple times
            var sumSquares = 0.0;

            for (var i = 0; i < items.Count; i++)
            {
                var val = criterion.ValueExtractor(items[i]);
                criterionValues[i] = val;

                sumSquares += (val * val);
            }

            var normalizedWeightFactor = criterion.Weight / sumWeight;
            var sqrtSumOfSquares = Math.Sqrt(sumSquares);

            // normalize and apply weight
            for (var j = 0; j < criterionValues.Length; j++)
            {
                var normalizedValue = criterionValues[j] / sqrtSumOfSquares;
                var weightedValue = normalizedValue * normalizedWeightFactor;

                criterionValues[j] = weightedValue;
            }

            // compute ideals, TOPSIS considers minimizing distance from best and maximizing distance from worst
            double idealWorst;
            double idealBest;

            if (criterion.Positivity == Positivity.Positive)
            {
                // if a criterion is a positive one, then the best value is the highest value
                idealWorst = criterionValues.Min();
                idealBest = criterionValues.Max();
            }
            else
            {
                // otherwise the best case is the lowest value
                idealWorst = criterionValues.Max();
                idealBest = criterionValues.Min();
            }

            ideals[c] = new ValuesIdeal(idealWorst, idealBest, criterionValues);
        }

        var results = new TopsisResult<T>[items.Count];
        for (var i = 0; i < items.Count; i++)
        {
            // distance is sqrt(sum of (delta from ideal)^2), we'll sqrt later so we can avoid multiple passes through the data
            var distanceFromBest = 0.0;
            var distanceFromWorst = 0.0;

            for (var c = 0; c < criteria.Count; c++)
            {
                var attr = ideals[c];
                distanceFromBest += Math.Pow(attr.CriterionValues[i] - attr.Best, 2);
                distanceFromWorst += Math.Pow(attr.CriterionValues[i] - attr.Worst, 2);
            }

            distanceFromBest = Math.Sqrt(distanceFromBest);
            distanceFromWorst = Math.Sqrt(distanceFromWorst);

            var score = distanceFromWorst / (distanceFromWorst + distanceFromBest);
            results[i] = new TopsisResult<T>(score, items[i]);
        }

        // sort in-place descending
        Array.Sort(results, (a, b) => b.Score.CompareTo(a.Score));
        return results;
    }
}
