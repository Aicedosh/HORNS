namespace HORNS
{
    /// <summary>
    /// Interfejs dla klas podlegających ocenie, tj. zmiennych i potrzeb.
    /// </summary>
    /// <typeparam name="T">Typ ocenianych danych.</typeparam>
    public interface IEvaluable<T>
    {
        /// <summary>
        /// Oblicza ocenę potrzeby dla danej wartości.
        /// </summary>
        /// <param name="value">Wartość do oceny.</param>
        /// <returns>Ocena dla danej wartości.</returns>
        float Evaluate(T value);
    }
}
