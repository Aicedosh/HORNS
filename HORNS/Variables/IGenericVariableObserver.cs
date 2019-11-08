using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    public interface IVariableObserver<T>
    {
        void ValueChanged(T value);
    }
}
