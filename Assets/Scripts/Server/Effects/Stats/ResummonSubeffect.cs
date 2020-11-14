using KompasCore.Effects;

namespace KompasServer.Effects
{
    public class ResummonSubeffect : ServerSubeffect
    {
        public override bool Resolve()
        {
            if (Target == null || Target.Location != CardLocation.Field)
                return ServerEffect.EffectImpossible();

            var ctxt = new ActivationContext(card: Target, stackable: Effect, triggerer: EffectController, space: Target.Position);
            ServerEffect.EffectsController.TriggerForCondition(Trigger.Play, ctxt);
            ServerEffect.EffectsController.TriggerForCondition(Trigger.Arrive, ctxt);
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}