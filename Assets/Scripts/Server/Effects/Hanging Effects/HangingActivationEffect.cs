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
            Effect sourceEff, ActivationContext currentContext, GameCard target, ServerSubeffect source)
            : base(serverGame, triggerRestriction, endCondition, fallOffCondition, fallOffRestriction, sourceEff, currentContext, removeIfEnd: true)
        {
            this.target = target != null ? target : throw new System.ArgumentNullException(nameof(target), "Cannot target a null card for a hanging activation");
            this.source = source ?? throw new System.ArgumentNullException(nameof(source), "Cannot make a hanging activation effect from no subeffect");
            target.SetActivated(true, source.ServerEffect);
        }

        public override void Resolve(ActivationContext context)
            => target.SetActivated(false, source.ServerEffect);
    }
}