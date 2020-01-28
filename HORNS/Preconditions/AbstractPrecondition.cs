using System.Collections.Generic;

namespace HORNS
{
    /// <summary>
    /// Abstrakcyjna klasa bazowa dla wszystkich wymagań.
    /// </summary>
    public abstract class Precondition : IIdentifiable
    {
        /// <summary>
        /// Typ wyliczeniowy służący do opisu wyniku porównania dwóch wymagań.
        /// </summary>
        public enum ComparisonResult
        {
            /// <summary>
            /// Wartość oznaczająca, że wymagań nie można porównać.
            /// </summary>
            NotComparable,
            /// <summary>
            /// Wartość oznaczająca, że dane wymaganie jest lepsze od wymagania, z którym jest ono porównywane.
            /// </summary>
            Better,
            /// <summary>
            /// Wartość oznaczająca, że dane wymaganie jest gorsze lub takie samo jak wymaganie, z którym jest ono porównywane.
            /// </summary>
            EqualWorse
        }

        internal int Id => GetVariable().Id;
        int IIdentifiable.Id => Id;

        /// <summary>
        /// Wykonuje kopię obiektu wymagania.
        /// </summary>
        /// <returns>Kopia wymagania.</returns>
        protected internal abstract Precondition Clone();
        IIdentifiable IIdentifiable.GetCopy()
        {
            return Clone();
        }
        
        internal abstract bool IsFulfilled();
        internal abstract bool IsFulfilledBy(IdSet<Variable> variables);
        internal abstract bool IsFulfilledByWorld();

        /// <summary>
        /// Porównuje wymaganie z innym wymaganiem.
        /// </summary>
        /// <param name="precondition">Wymaganie do porównania.</param>
        /// <returns>ComparisonResult.Better, jeżeli obecne wymaganie jest w lepszym (łatwiejszym do spełnienia) stanie niż precondition; ComparisonResult.EqualWorse, jeżeli jest w takim samym bądź gorszym stanie niż precondition; ComparisonResult.NotComparable, jeżeli nie jest możliwe porównanie wymagań.</returns>
        protected internal abstract ComparisonResult IsBetterThan(Precondition precondition);
        /// <summary>
        /// Wyznacza akcje mogące spełnić wymaganie.
        /// </summary>
        /// <param name="agent">Agent, którego akcje będą rozważane.</param>
        /// <returns>Kolekcja akcji.</returns>
        internal abstract IEnumerable<Action> GetActions(Agent agent);
        /// <summary>
        /// Łączy wymaganie z innym wymaganiem.
        /// </summary>
        /// <param name="precondition">Wymaganie do połączenia.</param>
        /// <returns>Nowe wymaganie będące wynikiem połączenia.</returns>
        protected internal abstract Precondition Combine(Precondition precondition);

        internal abstract Variable GetVariable();
        internal abstract Precondition Apply(ActionResult actionResult);
    }
}
