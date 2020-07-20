﻿using KompasCore.Effects;

namespace KompasServer.Effects
{
    public class DelaySubeffect : ServerSubeffect
    {
        public int numTimesToDelay = 0;
        public int indexToResume;
        public string triggerCondition;
        public TriggerRestriction triggerRestriction = new TriggerRestriction();

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            triggerRestriction.Initialize(this, ThisCard, null);
        }

        public override bool Resolve()
        {
            var eff = new DelayedHangingEffect(ServerGame, triggerRestriction, triggerCondition,
                numTimesToDelay, ServerEffect, indexToResume, EffectController);
            ServerGame.EffectsController.RegisterHangingEffect(triggerCondition, eff);
            return ServerEffect.EffectImpossible();
        }
    }
}