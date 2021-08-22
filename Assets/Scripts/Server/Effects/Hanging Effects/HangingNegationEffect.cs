using KompasCore.Cards;
using KompasCore.Effects;
using KompasServer.GameCore;

namespace KompasServer.Effects
{
    public class HangingNegationEffect : HangingEffect
    {
        private readonly GameCard target;
        private readonly ServerSubeffect source;

        public HangingNegationEffect(ServerGame serverGame, TriggerRestriction triggerRestriction, string endCondition, 
            string fallOffCondition, TriggerRestriction fallOffRestriction, ActivationContext currentContext,
            GameCard target, ServerSubeffect source)
            : base(serverGame, triggerRestriction, endCondition, fallOffCondition, fallOffRestriction, currentContext, removeIfEnd: false)
        {
            this.target = target != null ? target : throw new System.ArgumentNullException("target", "Cannot target a null card for a hanging negation");
            this.source = source ?? throw new System.ArgumentNullException("source", "Cannot make a hanging negation effect from no subeffect");
            target.SetNegated(true, source.ServerEffect);
        }

        protected override void Resolve()
        {
            target.SetNegated(false, source.ServerEffect);
        }
    }
}