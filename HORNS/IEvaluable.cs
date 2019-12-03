namespace HORNS
{
    public interface IEvaluable<T>
    {
        float Evaluate(T value);
    }
}
