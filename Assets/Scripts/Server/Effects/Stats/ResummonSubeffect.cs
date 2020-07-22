using KompasCore.Effects;

namespace KompasServer.Effects
{
    public class ResummonSubeffect : ServerSubeffect
    {
        public override bool Resolve()
        {
            if (Target == null) return ServerEffect.EffectImpossible();

            var ctxt = new ActivationContext(card: Target, stackable: Effect, triggerer: EffectController, space: Target.Position);
            ServerEffect.EffectsController.TriggerForCondition(Trigger.Play, ctxt);
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}