using KompasCore.Effects;
using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class ResummonSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (CardTarget == null)
                throw new NullCardException(TargetWasNull);
            else if (forbidNotBoard && CardTarget.Location != CardLocation.Board)
                throw new InvalidLocationException(CardTarget.Location, CardTarget, "Target not on board :(");

            var ctxt = new ActivationContext(mainCardBefore: CardTarget, stackable: Effect, player: EffectController, space: CardTarget.Position);
            ctxt.CacheCardInfoAfter();
            ServerEffect.EffectsController.TriggerForCondition(Trigger.Play, ctxt);
            ServerEffect.EffectsController.TriggerForCondition(Trigger.Arrive, ctxt);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}