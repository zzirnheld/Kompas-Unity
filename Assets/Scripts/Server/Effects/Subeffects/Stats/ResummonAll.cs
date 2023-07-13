using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Effects.Restrictions.GamestateRestrictionElements;
using KompasCore.GameCore;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
	public class ResummonAll : ServerSubeffect
	{
		public IRestriction<GameCardBase> cardRestriction;

		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			base.Initialize(eff, subeffIndex);
			cardRestriction ??= new AlwaysValid();
			cardRestriction.Initialize(DefaultInitializationContext);
		}

		public override Task<ResolutionInfo> Resolve()
		{
			foreach (var c in Game.BoardController.CardsWhere(c => cardRestriction.IsValid(c, ResolutionContext)))
			{
				var ctxt = new TriggeringEventContext(game: ServerGame, CardBefore: c, stackableCause: Effect, player: EffectController, space: c.Position);
				ctxt.CacheCardInfoAfter();
				ServerEffect.EffectsController.TriggerForCondition(Trigger.Play, ctxt);
				ServerEffect.EffectsController.TriggerForCondition(Trigger.Arrive, ctxt);
			}

			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}