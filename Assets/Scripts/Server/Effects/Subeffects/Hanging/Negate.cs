using System.Collections.Generic;
using KompasCore.Cards;
using KompasCore.Effects;
using KompasServer.GameCore;

namespace KompasServer.Effects.Subeffects.Hanging
{
    public class Negate : HangingEffectSubeffect
    {
        public bool negated = true;

        protected override IEnumerable<HangingEffect> CreateHangingEffects()
        {
            var contextCopy = CurrentContext.Copy;
            contextCopy.SetResumeInfo(Effect.CardTargets, Effect.SpaceTargets, Effect.stackableTargets,
                CardTarget, SpaceTarget, StackableTarget);
            var tempNegation = new NegationEffect(serverGame: ServerGame,
                                                         triggerRestriction: triggerRestriction,
                                                         endCondition: endCondition,
                                                         fallOffCondition: fallOffCondition,
                                                         fallOffRestriction: CreateFallOffRestriction(CardTarget),
                                                         currentContext: contextCopy,
                                                         target: CardTarget,
                                                         source: this,
                                                         negated: negated);
            return new List<HangingEffect>() { tempNegation };
        }

        public class NegationEffect : HangingEffect
        {
            private readonly GameCard target;
            private readonly ServerSubeffect source;
            private readonly bool negated;

            public NegationEffect(ServerGame serverGame, TriggerRestriction triggerRestriction, string endCondition,
                string fallOffCondition, TriggerRestriction fallOffRestriction,
                ActivationContext currentContext, GameCard target, ServerSubeffect source, bool negated)
                : base(serverGame, triggerRestriction, endCondition, fallOffCondition, fallOffRestriction, source.Effect, currentContext, removeIfEnd: false)
            {
                this.target = target ?? throw new System.ArgumentNullException(nameof(target), "Cannot target a null card for a hanging negation");
                this.source = source ?? throw new System.ArgumentNullException(nameof(source), "Cannot make a hanging negation effect from no subeffect");
                this.negated = negated;
                target.SetNegated(negated, source.Effect);
            }

            public override void Resolve(ActivationContext context) => target.SetNegated(!negated, source.Effect);
        }
    }
}