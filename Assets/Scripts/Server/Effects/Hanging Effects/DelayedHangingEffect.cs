using KompasCore.Effects;
using KompasServer.GameCore;

namespace KompasServer.Effects
{
    public class DelayedHangingEffect : HangingEffect
    {
        private readonly int numTimesToDelay;
        private int numTimesDelayed;
        private readonly ServerEffect toResume;
        private readonly int indexToResumeResolution;
        private readonly ServerPlayer controller;

        public DelayedHangingEffect(ServerGame game, TriggerRestriction triggerRestriction, string endCondition,
            int numTimesToDelay, ServerEffect toResume, int indexToResumeResolution, ServerPlayer controller)
            : base(game, triggerRestriction, endCondition)
        {
            this.numTimesToDelay = numTimesToDelay;
            this.toResume = toResume;
            this.indexToResumeResolution = indexToResumeResolution;
            this.controller = controller;
            numTimesDelayed = 0;
        }

        protected override bool ShouldEnd(ActivationContext context)
        {
            //first check any other logic
            if (!base.ShouldEnd(context)) return false;

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
            var context = new ActivationContext(startIndex: indexToResumeResolution);
            serverGame.EffectsController.PushToStack(toResume, controller, context);
        }
    }
}