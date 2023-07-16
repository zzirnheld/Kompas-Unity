using System.Collections.Generic;
using KompasCore.Cards;
using System.Linq;

namespace KompasCore.Effects.Restrictions.ListRestrictionElements
{
	public class ControllerCanPayCost : ListRestrictionElementBase
	{
		protected override bool IsValidLogic(IEnumerable<GameCardBase> item, IResolutionContext context)
			=> item.Select(c => c.Cost).Sum() <= InitializationContext.Controller.Pips;

		public override bool AllowsValidChoice(IEnumerable<GameCardBase> options, IResolutionContext context)
		{
			if (!(InitializationContext.parent is IListRestriction parent)) return true;

			//Accounts for all deduplicating of other possible things like distinct name, but doesn't check that there are enough (those deduplicators check that)
			return parent.Deduplicate(options)
				.Select(c => c.Cost)
				.OrderBy(c => c)
				.Take(parent.GetMinimum(context))
				.Sum() <= InitializationContext.Controller.Pips;
		}
	}
}