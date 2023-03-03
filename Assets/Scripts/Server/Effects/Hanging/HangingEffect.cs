using KompasCore.Effects;
using KompasServer.GameCore;
using UnityEngine;

namespace KompasServer.Effects.Hanging
{
    public abstract class HangingEffect
    {
        public readonly Effect sourceEff;
        public readonly string endCondition;
        public readonly string fallOffCondition;
        public readonly TriggerRestriction fallOffRestriction;

        public bool RemoveIfEnd { get; }

        private bool ended = false;
        private readonly TriggerRestriction triggerRestriction;
        protected readonly ServerGame serverGame;
        private readonly ActivationContext savedContext;

        public HangingEffect(ServerGame serverGame, TriggerRestriction triggerRestriction, string endCondition,
            string fallOffCondition, TriggerRestriction fallOffRestriction,
            Effect sourceEff, ActivationContext currentContext, bool removeIfEnd)
        {
            this.serverGame = serverGame != null ? serverGame : throw new System.ArgumentNullException(nameof(serverGame), "ServerGame in HangingEffect must not be null");
            this.triggerRestriction = triggerRestriction ?? throw new System.ArgumentNullException(nameof(triggerRestriction), "Trigger Restriction in HangingEffect must not be null");
            this.endCondition = endCondition;

            this.fallOffCondition = fallOffCondition;
            this.fallOffRestriction = fallOffRestriction;

            this.sourceEff = sourceEff;
            savedContext = currentContext;
            RemoveIfEnd = removeIfEnd;
        }

        public virtual bool ShouldBeCanceled(ActivationContext context) => fallOffRestriction.IsValidTriggeringContext(context);

        public virtual bool ShouldResolve(ActivationContext context)
        {
            //if we've already ended this hanging effect, we shouldn't end it again.
            if (ended) return false;
            Debug.Log($"Checking whether {this} should end for context {context}, with saved context {savedContext}");
            return triggerRestriction.IsValidTriggeringContext(context, secondary: savedContext);
        }

        /// <summary>
        /// Resolves the hanging effect.
        /// This usually amounts to canceling whatever effect the hanging effect originally applied,
        /// or maybe resuming a delayed effect.
        /// </summary>
        /// <param name="context">The context in which the effect is being resolved.</param>
        public abstract void Resolve(ActivationContext context);

        public override string ToString()
        {
            return $"{GetType()} ending when {endCondition}";
        }
    }
}