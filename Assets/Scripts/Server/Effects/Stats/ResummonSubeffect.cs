using KompasCore.Effects;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class ResummonSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (Target == null || Target.Location != CardLocation.Field)
                return Task.FromResult(ResolutionInfo.Impossible(TargetWasNull));

            var ctxt = new ActivationContext(card: Target, stackable: Effect, triggerer: EffectController, space: Target.Position);
            ServerEffect.EffectsController.TriggerForCondition(Trigger.Play, ctxt);
            ServerEffect.EffectsController.TriggerForCondition(Trigger.Arrive, ctxt);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}