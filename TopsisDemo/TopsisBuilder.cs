namespace TopsisDemo;

public enum Positivity
{
    Positive,
    Negative
}

public class Criteria<T>
{
    public Criteria(Func<T, double> valueExtractor, double weight, Positivity positivity)
    {
        ValueExtractor = valueExtractor;
        Weight = weight;
        Positivity = positivity;
    }

    public double Weight { get; }

    public Func<T, double> ValueExtractor { get; }

    public Positivity Positivity { get; }
}

public struct TopsisResult<T>
{
    public TopsisResult(double score, T item)
    {
        Score = score;
        Item = item;
    }

    public double Score { get; }
    public T Item { get; }
}

public class TopsisBuilder<T>
{
    private readonly List<Criteria<T>> _criteria = new List<Criteria<T>>();

    public TopsisBuilder<T> AddCriteria(Func<T, double> valueExtractor, double weight, Positivity positivity)
    {
        _criteria.Add(new Criteria<T>(valueExtractor, weight, positivity));
        return this;
    }

    public TopsisBuilder<T> AddCriteria(Func<T, int> valueExtractor, double weight, Positivity positivity)
    {
        _criteria.Add(new Criteria<T>((value) => valueExtractor(value), weight, positivity));
        return this;
    }

    public TopsisBuilder<T> AddCriteria(Func<T, decimal> valueExtractor, double weight, Positivity positivity)
    {
        _criteria.Add(new Criteria<T>((value) => (double)valueExtractor(value), weight, positivity));
        return this;
    }

    public TopsisBuilder<T> AddCriteria<TEnum>(Func<T, TEnum> valueExtractor, double weight, Positivity positivity)
        where TEnum : Enum
    {
        _criteria.Add(new Criteria<T>((value) => Convert.ToInt32(valueExtractor(value)), weight, positivity));
        return this;
    }

    public TopsisResult<T>[] Execute(IReadOnlyList<T> items)
    {
        return Topsis.Compute(_criteria, items);
    }
}
