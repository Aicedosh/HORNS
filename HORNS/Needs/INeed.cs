using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    /// <summary>
    /// Interfejs dla wszystkich potrzeb.
    /// </summary>
    public interface INeed
    {
        /// <summary>
        /// Oblicza ocenę obecnego stanu potrzeby.
        /// </summary>
        /// <returns>Ocena stanu potrzeby.</returns>
        float Evaluate();
        /// <summary>
        /// Sprawdza, czy potrzeba jest w stanie zaspokojonym.
        /// </summary>
        /// <returns>\texttt{true}, jeżeli potrzeba jest zaspokojona.</returns>
        bool IsSatisfied();
    }
}
