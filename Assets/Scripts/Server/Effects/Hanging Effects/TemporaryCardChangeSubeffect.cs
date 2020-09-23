using System.Collections.Generic;
using KompasCore.Effects;
using KompasCore.Cards;

namespace KompasServer.Effects
{
    [System.Serializable]
    public abstract class TemporaryCardChangeSubeffect : ServerSubeffect
    {
        public TriggerRestriction triggerRestriction = new TriggerRestriction();
        public string endCondition;
        public virtual bool ContinueResolution => true;

        public string fallOffCondition = Trigger.Remove;
        public string[] fallOffRestrictions =
        {
            TriggerRestriction.ThisCardTriggered,
            TriggerRestriction.ThisCardInPlay,
        };

        protected TriggerRestriction CreateFallOffRestriction(GameCard card)
        {
            //conditions for falling off
            var triggerRest = new TriggerRestriction() { triggerRestrictions = fallOffRestrictions };
            triggerRest.Initialize(this, card, null);
            return triggerRest;
        }

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            triggerRestriction.Initialize(this, ThisCard, null);
        }

        public override bool Resolve()
        {
            //create the hanging effects, of which there can be multiple
            var effectsApplied = CreateHangingEffects();

            //each of the effects needs to be registered, and registered for how it could fall off
            foreach (var eff in effectsApplied)
            {
                ServerGame.EffectsController.RegisterHangingEffect(endCondition, eff);
                if(!string.IsNullOrEmpty(fallOffCondition))
                    ServerGame.EffectsController.RegisterHangingEffectFallOff(fallOffCondition, eff.FallOffRestriction, eff);
            }

            //after all that's done, make it do the next subeffect
            if (ContinueResolution) return ServerEffect.ResolveNextSubeffect();
            else return ServerEffect.EndResolution();
        }

        protected abstract IEnumerable<HangingEffect> CreateHangingEffects();
    }
}