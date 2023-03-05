﻿using KompasCore.Cards;
using KompasCore.Cards.Movement;

namespace KompasServer.Effects.Subeffects
{
    public class Hand : ChangeGameLocation
    {
        protected override CardLocation destination => CardLocation.Hand;

        protected override void ChangeLocation(GameCard card) => card.Hand(card.Owner, Effect);
    }
}