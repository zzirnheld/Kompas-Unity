using KompasCore.Cards;
using KompasCore.Effects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
	public class CardTargetSaveRest : CardTarget
	{
		public IRestriction<GameCardBase> restRestriction;

		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			base.Initialize(eff, subeffIndex);
			if (restRestriction == null) restRestriction = cardRestriction;
			else restRestriction.Initialize(DefaultInitializationContext);
		}

		protected override Task<ResolutionInfo> NoPossibleTargets()
		{
			var rest = ServerGame.Cards.Where(c => restRestriction.IsValid(c, ResolutionContext));
			ServerEffect.rest.AddRange(rest);
			return base.NoPossibleTargets();
		}

		protected override void AddList(IEnumerable<GameCard> choices)
		{
			base.AddList(choices);
			var rest = toSearch.From(ResolutionContext, default)
				.Where(c => restRestriction.IsValid(c, ResolutionContext) && !choices.Contains(c))
				.Select(c => c.Card);
			ServerEffect.rest.AddRange(rest);
		}
	}
}