using KompasCore.Effects;
using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class ResummonSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (Target == null)
                throw new NullCardException(TargetWasNull);
            else if (forbidNotBoard && Target.Location != CardLocation.Field)
                throw new InvalidLocationException(Target.Location, Target, "Target not on board :(");

            var ctxt = new ActivationContext(mainCardBefore: Target, stackable: Effect, player: EffectController, space: Target.Position);
            ctxt.CacheCardInfoAfter(Target);
            ServerEffect.EffectsController.TriggerForCondition(Trigger.Play, ctxt);
            ServerEffect.EffectsController.TriggerForCondition(Trigger.Arrive, ctxt);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}