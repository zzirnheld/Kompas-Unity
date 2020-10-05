using KompasCore.Effects;
using KompasCore.Cards;
using KompasServer.GameCore;

namespace KompasServer.Effects
{
    public class HangingActivationEffect : HangingEffect
    {
        private readonly GameCard target;
        private readonly ServerSubeffect source;

        public HangingActivationEffect(ServerGame serverGame, TriggerRestriction triggerRestriction, string endCondition, 
            string fallOffCondition, TriggerRestriction fallOffRestriction,
            GameCard target, ServerSubeffect source)
            : base(serverGame, triggerRestriction, endCondition, fallOffCondition, fallOffRestriction)
        {
            this.target = target != null ? target : throw new System.ArgumentNullException("target", "Cannot target a null card for a hanging activation");
            this.source = source ?? throw new System.ArgumentNullException("source", "Cannot make a hanging activation effect from no subeffect");
            target.SetActivated(true, source.ServerEffect);
        }

        protected override void Resolve()
        {
            target.SetActivated(false, source.ServerEffect);
        }
    }
}