using System;
using System.Collections.Generic;
using System.Text;

namespace HORNS
{
    internal class RequirementSet : IdSet<Requirement>
    {
        internal override bool Add(Requirement r)
        {
            if (elements.TryGetValue(r.Id, out Requirement existing))
            {
                r = r.Combine(existing);
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
