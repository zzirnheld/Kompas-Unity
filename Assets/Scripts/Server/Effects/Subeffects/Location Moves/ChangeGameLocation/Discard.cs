﻿using KompasCore.Cards;
using KompasCore.Cards.Movement;
using KompasCore.Effects;

namespace KompasServer.Effects.Subeffects
{
	public class Discard : ChangeGameLocation
	{
		protected override CardLocation destination => CardLocation.Discard;

		protected override void ChangeLocation(GameCard card) => card.Discard(Effect);
	}

	public class Vanish : Discard
	{
		protected override void ChangeLocation(GameCard card)
		{
			TriggeringEventContext context = new TriggeringEventContext(game: ServerGame, CardBefore: card);
			base.ChangeLocation(card);
			context.CacheCardInfoAfter();
			ServerEffect.EffectsController.TriggerForCondition(Trigger.Vanish, context);
		}
    }
}