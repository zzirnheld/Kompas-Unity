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
			if (!(InitializationContext.parent is AllOf parent)) return true;

			int min = parent.elements
				.Where(elem => elem is Minimum)
				.Select(min => min as Minimum)
				.Select(min => min.StashBound(context))
				.DefaultIfEmpty(0)
				.Max(); //We want the highest (i.e. most constraining) lower bound

			//Accounts for all deduplicating of other possible things like distinct name, but doesn't check that there are enough (those deduplicators check that)
			return parent.Deduplicate(options)
				.Select(c => c.Cost)
				.OrderBy(c => c)
				.Take(min)
				.Sum() <= InitializationContext.Controller.Pips;
		}
	}
}