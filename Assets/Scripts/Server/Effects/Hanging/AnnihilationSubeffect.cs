using System.Collections.Generic;
using KompasCore.Cards;
using KompasCore.Cards.Movement;
using KompasCore.Effects;
using KompasServer.GameCore;

namespace KompasServer.Effects.Hanging
{
    public class AnnihilationSubeffect : HangingEffectSubeffect
    {
        protected override IEnumerable<HangingEffect> CreateHangingEffects()
        {
            var contextCopy = CurrentContext.Copy;
            contextCopy.SetResumeInfo(Effect.CardTargets, Effect.SpaceTargets, Effect.stackableTargets,
                CardTarget, SpaceTarget, StackableTarget);
            var eff = new Annihilation(serverGame: ServerGame,
                                                    triggerRestriction: triggerRestriction,
                                                    endCondition: endCondition,
                                                    fallOffCondition: fallOffCondition,
                                                    fallOffRestriction: CreateFallOffRestriction(CardTarget),
                                                    sourceEff: Effect,
                                                    currentContext: contextCopy,
                                                    target: CardTarget);
            return new List<HangingEffect>() { eff };
        }

        /// <summary>
        /// Does nothing when created. When resolves, annihilates its target
        /// </summary>
        private class Annihilation : HangingEffect
        {
            private readonly GameCard target;

            public Annihilation(ServerGame serverGame, TriggerRestriction triggerRestriction, string endCondition,
                string fallOffCondition, TriggerRestriction fallOffRestriction,
                Effect sourceEff, ActivationContext currentContext, GameCard target)
                : base(serverGame, triggerRestriction, endCondition, fallOffCondition, fallOffRestriction, sourceEff, currentContext, removeIfEnd: true)
            {
                this.target = target;
            }

            public override void Resolve(ActivationContext context) => target.Annihilate(sourceEff);

            public override string ToString()
            {
                return $"{base.ToString()} affecting {target}";
            }
        }
    }
}