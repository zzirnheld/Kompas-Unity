using KompasCore.Effects;
using KompasCore.GameCore;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class ResummonAllSubeffect : ServerSubeffect
    {
        public CardRestriction cardRestriction;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            cardRestriction = cardRestriction ?? new CardRestriction();
            cardRestriction.Initialize(this);
        }

        public override Task<ResolutionInfo> Resolve()
        {
            foreach (var c in Game.boardCtrl.CardsWhere(c => cardRestriction.Evaluate(c)))
            {
                var ctxt = new ActivationContext(card: c, stackable: Effect, triggerer: EffectController, space: c.Position);
                ServerEffect.EffectsController.TriggerForCondition(Trigger.Play, ctxt);
                ServerEffect.EffectsController.TriggerForCondition(Trigger.Arrive, ctxt);
            }

            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}