using KompasCore.Effects;
using KompasServer.GameCore;
using UnityEngine;

namespace KompasServer.Effects
{
    public abstract class HangingEffect
    {
        public readonly Effect sourceEff;
        public readonly string endCondition;
        public readonly string fallOffCondition;
        public readonly TriggerRestriction fallOffRestriction;

        private bool ended = false;
        private readonly TriggerRestriction triggerRestriction;
        protected readonly ServerGame serverGame;
        private readonly ActivationContext savedContext;
        private readonly bool removeIfEnd;

        public HangingEffect(ServerGame serverGame, TriggerRestriction triggerRestriction, string endCondition, 
            string fallOffCondition, TriggerRestriction fallOffRestriction, 
            Effect sourceEff, ActivationContext currentContext, bool removeIfEnd)
        {
            this.serverGame = serverGame != null ? serverGame : throw new System.ArgumentNullException("serverGame", "ServerGame in HangingEffect must not be null");
            this.triggerRestriction = triggerRestriction ?? throw new System.ArgumentNullException("triggerRestriction", "Trigger Restriction in HangingEffect must not be null");
            this.endCondition = endCondition;

            this.fallOffCondition = fallOffCondition;
            this.fallOffRestriction = fallOffRestriction;

            this.sourceEff = sourceEff;
            savedContext = currentContext;
            this.removeIfEnd = removeIfEnd;
        }

        /// <summary>
        /// Called when the trigger for this hanging effect occurs.
        /// If the hanging effect should end (as determined by the ShouldEnd function),
        /// ends the hanging effect (as determined by Resolve)
        /// </summary>
        /// <param name="cardTrigger">The card that triggered the triggering event</param>
        /// <param name="stackTrigger">The item on the stack that triggered the event</param>
        /// <returns><see langword="true"/> if the hanging effect is now ended as a result of this call, <see langword="false"/> otherwise.</returns>
        public virtual bool EndIfApplicable(ActivationContext context)
        {
            //check now if we should end it. store that result in ended, because if we did end already, we shouldn't end again
            bool shouldResolve = ShouldResolve(context);
            //if we should end it, resolve the way to end this hanging effect
            if (shouldResolve) Resolve(context);
            ended = shouldResolve && removeIfEnd;
            //then return whether the effect ended. note that if the effect already ended, this will return false
            return ended;
        }

        public virtual bool ShouldBeCanceled(ActivationContext context) => fallOffRestriction.IsValidTriggeringContext(context);

        protected virtual bool ShouldResolve(ActivationContext context)
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
        protected abstract void Resolve(ActivationContext context);

        public override string ToString()
        {
            return $"{GetType()} ending when {endCondition}";
        }
    }
}