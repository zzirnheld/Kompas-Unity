using KompasCore.Cards;
using KompasCore.Effects;
using KompasServer.GameCore;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasServer.Effects.Subeffect.Hanging
{
    public class DelaySubeffect : HangingEffectSubeffect
    {
        public int numTimesToDelay = 0;
        public bool clearWhenResume = true;
        public override bool ContinueResolution => false;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            if (jumpIndices == null) throw new System.ArgumentNullException(nameof(jumpIndices));
        }

        protected override IEnumerable<HangingEffect> CreateHangingEffects()
        {
            Debug.Log($"Is context null? {CurrentContext == null}");
            Debug.Log($"Are jump indices null? {jumpIndices == null}");
            ActivationContext contextCopy = CurrentContext.Copy;
            contextCopy.SetResumeInfo(Effect.CardTargets, Effect.SpaceTargets, Effect.stackableTargets,
                CardTarget, SpaceTarget, StackableTarget,
                JumpIndex);
            var delay = new Delay(game: ServerGame,
                                                 triggerRestriction: triggerRestriction,
                                                 endCondition: endCondition,
                                                 fallOffCondition: fallOffCondition,
                                                 fallOffRestriction: CreateFallOffRestriction(Source),
                                                 sourceEff: Effect,
                                                 currentContext: contextCopy,
                                                 numTimesToDelay: numTimesToDelay,
                                                 toResume: ServerEffect,
                                                 indexToResumeResolution: JumpIndex,
                                                 controller: EffectController,
                                                 targets: new List<GameCard>(Effect.CardTargets),
                                                 spaces: new List<Space>(Effect.SpaceTargets),
                                                 clearIfResolve: clearWhenResume);
            return new List<HangingEffect>() { delay };
        }

        private class Delay : HangingEffect
        {
            private readonly int numTimesToDelay;
            private int numTimesDelayed;
            private readonly ServerEffect toResume;
            private readonly int indexToResumeResolution;
            private readonly ServerPlayer controller;
            private readonly List<GameCard> targets;
            private readonly List<Space> spaces;

            public Delay(ServerGame game, TriggerRestriction triggerRestriction, string endCondition,
                string fallOffCondition, TriggerRestriction fallOffRestriction, Effect sourceEff, ActivationContext currentContext,
                int numTimesToDelay, ServerEffect toResume, int indexToResumeResolution, ServerPlayer controller,
                IEnumerable<GameCard> targets, IEnumerable<Space> spaces,
                bool clearIfResolve)
                : base(game, triggerRestriction, endCondition, fallOffCondition, fallOffRestriction, sourceEff, currentContext, clearIfResolve)
            {
                this.numTimesToDelay = numTimesToDelay;
                this.toResume = toResume;
                this.indexToResumeResolution = indexToResumeResolution;
                this.controller = controller;
                Debug.Log($"Targets are {string.Join(",", targets?.Select(c => c.ToString()) ?? new string[] { "Null" })}");
                this.targets = new List<GameCard>(targets);
                this.spaces = new List<Space>(spaces);
                numTimesDelayed = 0;
            }

            public override bool ShouldResolve(ActivationContext context)
            {
                UnityEngine.Debug.Log($"Checking if delayed hanging effect should end for context {context}, {numTimesDelayed}/{numTimesToDelay}");
                //first check any other logic
                if (!base.ShouldResolve(context)) return false;

                //if it should otherwise be fine, but we haven't waited enough times, delay further
                if (numTimesDelayed < numTimesToDelay)
                {
                    numTimesDelayed++;
                    return false;
                }
                else
                {
                    numTimesDelayed = 0;
                    return true;
                }
            }

            public override void Resolve(ActivationContext context)
            {
                var myContext = context.Copy;
                myContext.SetResumeInfo(targets, spaces, default, default, default, default, indexToResumeResolution);
                serverGame.effectsController.PushToStack(toResume, controller, myContext);
            }
        }
    }
}