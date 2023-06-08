using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.GameCore;
using KompasServer.Effects;
using KompasServer.Networking;

namespace KompasServer.GameCore
{
	public class ServerDiscardController : DiscardController
	{
		public ServerPlayer owner;

		public override Player Owner => owner;
		
		public ServerGame ServerGame => owner.game;
		public ServerNotifier ServerNotifier => ServerGame.serverPlayers[Owner.index].notifier;
		public ServerEffectsController EffectsController => ServerGame.effectsController;

		public override bool Discard(GameCard card, IStackable stackSrc = null)
		{
			GameCard cause = null;
			if (stackSrc is Effect eff) cause = eff.Source;
			else if (stackSrc is Attack atk)
			{
				if (atk.attacker == card) cause = atk.defender;
				else if (atk.defender == card) cause = atk.attacker;
				else if (atk.attacker == card.AugmentedCard) cause = atk.defender;
				else if (atk.defender == card.AugmentedCard) cause = atk.attacker;
				else throw new System.ArgumentException($"Why is {card} neither the attacker nor defender, nor augmenting them, " +
					$"in the attack {atk} that caused it to be discarded?");
			}
			var context = new TriggeringEventContext(game: ServerGame, CardBefore: card, secondaryCardBefore: cause, stackableCause: stackSrc, player: Owner);
			bool wasKnown = card.KnownToEnemy;
			bool successful = base.Discard(card, stackSrc);
			if (successful)
			{
				ServerNotifier.NotifyDiscard(card, wasKnown);
				context.CacheCardInfoAfter();
				EffectsController.TriggerForCondition(Trigger.Discard, context);
			}
			return successful;
		}
		
		public override void Refresh() { }
	}
}