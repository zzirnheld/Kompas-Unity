using System.Collections.Generic;
using KompasCore.Cards;
using KompasCore.Effects;
using KompasServer.GameCore;

namespace KompasServer.Effects.Subeffects.Hanging
{
    public class ActivationSubeffect : HangingEffectSubeffect
    {
        protected override IEnumerable<HangingEffect> CreateHangingEffects()
        {
            var contextCopy = CurrentContext.Copy;
            contextCopy.SetResumeInfo(Effect.CardTargets, Effect.SpaceTargets, Effect.stackableTargets,
                CardTarget, SpaceTarget, StackableTarget);
            var tempActivation = new Activation(serverGame: ServerGame,
                                                             triggerRestriction: triggerRestriction,
                                                             endCondition: endCondition,
                                                             fallOffCondition: fallOffCondition,
                                                             fallOffRestriction: CreateFallOffRestriction(CardTarget),
                                                             sourceEff: Effect,
                                                             currentContext: contextCopy,
                                                             target: CardTarget,
                                                             source: this);
            return new List<HangingEffect>() { tempActivation };
        }
        
        private class Activation : HangingEffect
        {
            private readonly GameCard target;
            private readonly ServerSubeffect source;

            public Activation(ServerGame serverGame, TriggerRestriction triggerRestriction, string endCondition,
                string fallOffCondition, TriggerRestriction fallOffRestriction,
                Effect sourceEff, ActivationContext currentContext, GameCard target, ServerSubeffect source)
                : base(serverGame, triggerRestriction, endCondition, fallOffCondition, fallOffRestriction, sourceEff, currentContext, removeIfEnd: true)
            {
                this.target = target != null ? target : throw new System.ArgumentNullException(nameof(target), "Cannot target a null card for a hanging activation");
                this.source = source ?? throw new System.ArgumentNullException(nameof(source), "Cannot make a hanging activation effect from no subeffect");
                target.SetActivated(true, source.ServerEffect);
            }

            public override void Resolve(ActivationContext context)
                => target.SetActivated(false, source.ServerEffect);
        }
    }
}