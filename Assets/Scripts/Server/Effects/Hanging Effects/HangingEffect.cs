using KompasCore.Effects;
using KompasServer.GameCore;

namespace KompasServer.Effects
{
    public abstract class HangingEffect
    {
        public readonly string EndCondition;

        private bool ended = false;
        private readonly TriggerRestriction triggerRestriction;
        protected readonly ServerGame serverGame;

        public HangingEffect(ServerGame serverGame, TriggerRestriction triggerRestriction, string endCondition)
        {
            this.serverGame = serverGame ?? throw new System.ArgumentNullException("ServerGame in HangingEffect must not be null");
            this.triggerRestriction = triggerRestriction ?? throw new System.ArgumentNullException("Trigger Restriction in HangingEffect must not be null");
            EndCondition = endCondition;
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
            ended = ShouldEnd(context);
            //if we should end it, resolve the way to end this hanging effect
            if (ended) Resolve();
            //then return whether the effect ended. note that if the effect already ended, this will return false
            return ended;
        }

        protected virtual bool ShouldEnd(ActivationContext context)
        {
            //if we've already ended this hanging effect, we shouldn't end it again.
            if (ended) return false;
            return triggerRestriction.Evaluate(context);
        }

        protected abstract void Resolve();
    }
}