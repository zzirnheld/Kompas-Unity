using System.Collections.Generic;
using KompasCore.Effects;
using KompasCore.Cards;
using System.Threading.Tasks;
using System.Linq;

namespace KompasServer.Effects.Subeffects.Hanging
{
    public abstract class HangingEffectSubeffect : ServerSubeffect
    {
        //BEWARE: once per turn might not work for these as impl rn, because it's kind of ill-defined.
        //this is only a problem if I one day start creating hanging effects that can later trigger once each turn.
        public TriggerRestriction triggerRestriction;
        public string endCondition;
        public virtual bool ContinueResolution => true;

        public string fallOffCondition = Trigger.Remove;
        public TriggerRestriction fallOffRestriction;

        protected TriggerRestriction CreateFallOffRestriction(GameCard card)
        {
            //conditions for falling off
            TriggerRestriction triggerRest = fallOffRestriction;
            if (triggerRest == null)
            {
                triggerRest = fallOffCondition == Trigger.Remove ?
                    new TriggerRestriction() { triggerRestrictionElements = TriggerRestriction.DefaultFallOffRestrictions } :
                    new TriggerRestriction() { triggerRestrictionElements = { } };
            }
            triggerRest.Initialize(DefaultInitializationContext);
            return triggerRest;
        }

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            triggerRestriction ??= new TriggerRestriction();
            triggerRestriction.Initialize(DefaultInitializationContext);

            if (TriggerRestriction.ReevalationRestrictions
                .Intersect(triggerRestriction.elements.Select(elem => elem.GetType()))
                .Any())
            {
                //TODO: test this. it might be that since they're pushed back to the stack it works fine,
                //but then I need to make sure there's no collision between resume 1/turn and initial trigger 1/turn.
                throw new System.ArgumentException("Hanging effect conditions might not currently support once per turns," +
                    "or any other restriction type that would need to be reevaluated after being pushed to stack!");
            }
        }

        public override Task<ResolutionInfo> Resolve()
        {
            //create the hanging effects, of which there can be multiple
            var effectsApplied = CreateHangingEffects();

            //each of the effects needs to be registered, and registered for how it could fall off
            foreach (var eff in effectsApplied)
            {
                ServerGame.effectsController.RegisterHangingEffect(endCondition, eff, fallOffCondition);
            }

            //after all that's done, make it do the next subeffect
            if (ContinueResolution) return Task.FromResult(ResolutionInfo.Next);
            else return Task.FromResult(ResolutionInfo.End(EndOnPurpose));
        }

        protected abstract IEnumerable<HangingEffect> CreateHangingEffects();
    }
}