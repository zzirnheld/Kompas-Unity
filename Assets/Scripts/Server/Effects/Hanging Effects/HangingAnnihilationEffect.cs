using KompasCore.Cards;
using KompasCore.Effects;
using KompasServer.GameCore;

namespace KompasServer.Effects
{
    /// <summary>
    /// Does nothing when created. When resolves, annihilates its target
    /// </summary>
    public class HangingAnnihilationEffect : HangingEffect
    {
        private readonly GameCard target;

        public HangingAnnihilationEffect(ServerGame serverGame, TriggerRestriction triggerRestriction, string endCondition, 
            string fallOffCondition, TriggerRestriction fallOffRestriction, ActivationContext currentContext,
            GameCard target)
            : base(serverGame, triggerRestriction, endCondition, fallOffCondition, fallOffRestriction, currentContext, removeIfEnd: false)
        {
            this.target = target;
        }

        protected override void Resolve()
        {
            target.Owner.annihilationCtrl.Annihilate(target);
        }

        public override string ToString()
        {
            return $"{base.ToString()} affecting {target}";
        }
    }
}