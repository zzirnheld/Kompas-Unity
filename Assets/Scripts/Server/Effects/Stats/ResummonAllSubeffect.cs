using KompasCore.Effects;
using KompasCore.GameCore;

namespace KompasServer.Effects
{
    public class ResummonAllSubeffect : ServerSubeffect
    {
        public CardRestriction cardRestriction = new CardRestriction();

        public override bool Resolve()
        {
            foreach (var c in Game.boardCtrl.CardsWhere(c => cardRestriction.Evaluate(c)))
            {
                var ctxt = new ActivationContext(card: c, stackable: Effect, triggerer: EffectController, space: Target.Position);
                ServerEffect.EffectsController.TriggerForCondition(Trigger.Play, ctxt);
            }

            return ServerEffect.EffectImpossible();
        }
    }
}