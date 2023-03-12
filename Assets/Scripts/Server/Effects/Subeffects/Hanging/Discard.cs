using System.Collections.Generic;
using KompasCore.Cards;
using KompasCore.Cards.Movement;
using KompasCore.Effects;
using KompasServer.GameCore;

namespace KompasServer.Effects.Subeffects.Hanging
{
    public class Discard : HangingEffectSubeffect
    {
        protected override IEnumerable<HangingEffect> CreateHangingEffects()
        {
            var contextCopy = CurrentContext.Copy;
            contextCopy.SetResumeInfo(Effect.CardTargets, Effect.SpaceTargets, Effect.stackableTargets,
                CardTarget, SpaceTarget, StackableTarget);
            var eff = new DiscardEffect(serverGame: ServerGame,
                                               triggerRestriction: triggerRestriction,
                                               endCondition: endCondition,
                                               fallOffCondition: fallOffCondition,
                                               sourceEff: Effect,
                                               fallOffRestriction: CreateFallOffRestriction(CardTarget),
                                               currentContext: contextCopy,
                                               target: CardTarget);
            return new List<HangingEffect>() { eff };
        }

        /// <summary>
        /// Does nothing when created. When resolves, annihilates its target
        /// </summary>
        private class DiscardEffect : HangingEffect
        {
            private readonly GameCard target;

            public DiscardEffect(ServerGame serverGame, TriggerRestriction triggerRestriction, string endCondition,
                string fallOffCondition, TriggerRestriction fallOffRestriction,
                Effect sourceEff, ActivationContext currentContext, GameCard target)
                : base(serverGame, triggerRestriction, endCondition, fallOffCondition, fallOffRestriction, sourceEff, currentContext, removeIfEnd: false)
            {
                this.target = target;
            }

            public override void Resolve(ActivationContext context)
                => target.Discard(sourceEff);
        }
    }
}