using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    internal class PreconditionSet : IdSet<Precondition>
    {
        internal override bool Add(Precondition r)
        {
            if (elements.TryGetValue(r.Id, out Precondition existing))
            {
                r = existing.Combine(r);
                if (r == null)
                {
                    return false;
                }
            }
            elements[r.Id] = r;
            return true;
        }
    }
}
