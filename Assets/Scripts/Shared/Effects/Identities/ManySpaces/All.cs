using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities.ManySpaces
{
    public class All : ContextlessLeafIdentityBase<IReadOnlyCollection<Space>>
    {
        protected override IReadOnlyCollection<Space> AbstractItem => Space.Spaces.ToList();
    }
}