using System.Collections.Generic;
using KompasCore.Effects;
using KompasCore.Cards;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects
{
    public abstract class HangingEffectSubeffect : ServerSubeffect
    {
        public TriggerRestriction triggerRestriction;
        public string endCondition;
        public virtual bool ContinueResolution => true;

        public string fallOffCondition = Trigger.Remove;
        public string[] fallOffRestrictions = TriggerRestriction.DefaultFallOffRestrictions;

        protected TriggerRestriction CreateFallOffRestriction(GameCard card)
        {
            //conditions for falling off
            TriggerRestriction triggerRest = null;
            //TODO have the ability for a player to configure a custom fall off restriction
            if (triggerRest == null)
            {
                triggerRest = fallOffCondition == Trigger.Remove ?
                    new TriggerRestriction() { triggerRestrictions = fallOffRestrictions } :
                    new TriggerRestriction() { triggerRestrictions = { } };
            }
            triggerRest.Initialize(Game, card, thisTrigger: null, effect: Effect);
            return triggerRest;
        }

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            triggerRestriction ??= new TriggerRestriction();
            triggerRestriction.Initialize(Game, ThisCard, thisTrigger: null, effect: Effect, subeffect: this);
            //Debug.LogWarning($"Are jump indices null? {jumpIndices == null}");
        }

        public override Task<ResolutionInfo> Resolve()
        {
            //create the hanging effects, of which there can be multiple
            var effectsApplied = CreateHangingEffects();

            //each of the effects needs to be registered, and registered for how it could fall off
            foreach (var eff in effectsApplied)
            {
                ServerGame.EffectsController.RegisterHangingEffect(endCondition, eff, fallOffCondition);
            }

            //after all that's done, make it do the next subeffect
            if (ContinueResolution) return Task.FromResult(ResolutionInfo.Next);
            else return Task.FromResult(ResolutionInfo.End(EndOnPurpose));
        }

        protected abstract IEnumerable<HangingEffect> CreateHangingEffects();
    }
}