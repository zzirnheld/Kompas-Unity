using KompasCore.Cards;
using KompasCore.Effects;
using KompasServer.GameCore;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasServer.Effects
{
    public class DelayedHangingEffect : HangingEffect
    {
        private readonly int numTimesToDelay;
        private int numTimesDelayed;
        private readonly ServerEffect toResume;
        private readonly int indexToResumeResolution;
        private readonly ServerPlayer controller;
        private readonly List<GameCard> targets;

        public DelayedHangingEffect(ServerGame game, TriggerRestriction triggerRestriction, string endCondition,
            string fallOffCondition, TriggerRestriction fallOffRestriction, ActivationContext currentContext,
            int numTimesToDelay, ServerEffect toResume, int indexToResumeResolution, ServerPlayer controller, IEnumerable<GameCard> targets, 
            bool clearIfResolve)
            : base(game, triggerRestriction, endCondition, fallOffCondition, fallOffRestriction, currentContext, clearIfResolve)
        {
            this.numTimesToDelay = numTimesToDelay;
            this.toResume = toResume;
            this.indexToResumeResolution = indexToResumeResolution;
            this.controller = controller;
            Debug.Log($"Targets are {string.Join(",", targets?.Select(c => c.ToString()) ?? new string[] { "Null" })}");
            this.targets = new List<GameCard>(targets);
            Debug.Log($"Clear when resume? {clearIfResolve}");
            numTimesDelayed = 0;
        }

        protected override bool ShouldResolve(ActivationContext context)
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

            numTimesDelayed = 0;
            return true;
        }

        protected override void Resolve()
        {
            Debug.Log($"Resuming {this}");
            var context = new ActivationContext(startIndex: indexToResumeResolution, targets: targets);
            serverGame.EffectsController.PushToStack(toResume, controller, context);
        }
    }
}