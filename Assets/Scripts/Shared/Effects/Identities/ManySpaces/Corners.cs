using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities.ManySpaces
{
	public class Corners : ContextlessLeafIdentityBase<IReadOnlyCollection<Space>>
	{
		protected override IReadOnlyCollection<Space> AbstractItem => Space.Spaces.Where(s => s.IsCorner).ToArray();
	}
}