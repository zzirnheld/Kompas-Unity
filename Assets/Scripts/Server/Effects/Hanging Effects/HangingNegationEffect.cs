using KompasCore.Cards;
using KompasCore.Effects;
using KompasServer.GameCore;

namespace KompasServer.Effects
{
    public class HangingNegationEffect : HangingEffect
    {
        private readonly GameCard target;
        private readonly ServerSubeffect source;
        private readonly bool negated;

        public HangingNegationEffect(ServerGame serverGame, TriggerRestriction triggerRestriction, string endCondition, 
            string fallOffCondition, TriggerRestriction fallOffRestriction, 
            ActivationContext currentContext, GameCard target, ServerSubeffect source, bool negated)
            : base(serverGame, triggerRestriction, endCondition, fallOffCondition, fallOffRestriction, source.Effect, currentContext, removeIfEnd: false)
        {
            this.target = target ?? throw new System.ArgumentNullException("target", "Cannot target a null card for a hanging negation");
            this.source = source ?? throw new System.ArgumentNullException("source", "Cannot make a hanging negation effect from no subeffect");
            this.negated = negated;
            target.SetNegated(negated, source.Effect);
        }

        protected override void Resolve(ActivationContext context) => target.SetNegated(!negated, source.Effect);
    }
}