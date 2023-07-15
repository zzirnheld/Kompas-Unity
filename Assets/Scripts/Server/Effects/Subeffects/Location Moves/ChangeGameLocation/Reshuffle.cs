﻿using KompasCore.Cards;
using KompasCore.Cards.Movement;

namespace KompasServer.Effects.Subeffects
{
	public class Reshuffle : ChangeGameLocation
	{
		public override bool IsImpossible(TargetingContext overrideContext = null)
			=> GetCardTarget(overrideContext) == null;
		protected override CardLocation destination => CardLocation.Deck;

		protected override void ChangeLocation(GameCard card) => card.Reshuffle(card.Owner, Effect);
	}
}