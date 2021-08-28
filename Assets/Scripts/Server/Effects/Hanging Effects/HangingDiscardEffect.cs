using KompasCore.Cards;
using KompasCore.Effects;
using KompasServer.GameCore;

namespace KompasServer.Effects
{
    /// <summary>
    /// Does nothing when created. When resolves, annihilates its target
    /// </summary>
    public class HangingDiscardEffect : HangingEffect
    {
        private readonly GameCard target;

        public HangingDiscardEffect(ServerGame serverGame, TriggerRestriction triggerRestriction, string endCondition,
            string fallOffCondition, TriggerRestriction fallOffRestriction, ActivationContext currentContext,
            GameCard target)
            : base(serverGame, triggerRestriction, endCondition, fallOffCondition, fallOffRestriction, currentContext, removeIfEnd: false)
        {
            this.target = target;
        }

        protected override void Resolve()
        {
            target.Discard();
        }
    }
}