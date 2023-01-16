using KompasCore.Cards;
using KompasCore.Cards.Movement;
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
            string fallOffCondition, TriggerRestriction fallOffRestriction,
            Effect sourceEff, ActivationContext currentContext, GameCard target)
            : base(serverGame, triggerRestriction, endCondition, fallOffCondition, fallOffRestriction, sourceEff, currentContext, removeIfEnd: true)
        {
            this.target = target;
        }

        public override void Resolve(ActivationContext context) => target.Annihilate(sourceEff);

        public override string ToString()
        {
            return $"{base.ToString()} affecting {target}";
        }
    }
}