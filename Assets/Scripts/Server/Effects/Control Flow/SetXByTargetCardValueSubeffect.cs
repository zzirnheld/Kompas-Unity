﻿using KompasCore.Effects;

namespace KompasServer.Effects
{
    public class SetXByTargetCardValueSubeffect : SetXSubeffect
    {
        public CardValue cardValue;
        //If true, gets value of target cardInfo. If false, gets value of target card
        public bool cardInfo = false;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            cardValue?.Initialize(DefaultRestrictionContext);
        }

        public override int BaseCount => cardInfo ?
            cardValue.GetValueOf(CardInfoTarget) :
            cardValue.GetValueOf(CardTarget);
    }
}