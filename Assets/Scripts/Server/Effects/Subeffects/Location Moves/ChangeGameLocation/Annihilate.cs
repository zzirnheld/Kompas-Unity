﻿using KompasCore.Cards;
using KompasCore.Cards.Movement;

namespace KompasServer.Effects.Subeffects
{
	public class Annihilate : ChangeGameLocation
	{
		protected override CardLocation destination => CardLocation.Annihilation;

		protected override void ChangeLocation(GameCard card) => card.Annihilate(Effect);
	}
}