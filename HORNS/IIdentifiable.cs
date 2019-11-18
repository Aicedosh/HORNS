using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    internal interface IIdentifiable
    {
        int Id { get; }

        IIdentifiable GetCopy();
    }
}
