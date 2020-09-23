namespace KompasServer.Effects
{
    public class AnnihilateSubeffect : CardChangeStateSubeffect
    {
        public override bool Resolve()
        {
            if (Target == null || Target.Location == CardLocation.Annihilation) return ServerEffect.EffectImpossible();
            else
            {
                if (Game.annihilationCtrl.Annihilate(Target, Effect)) return ServerEffect.ResolveNextSubeffect();
                else return ServerEffect.EffectImpossible();
            }
        }
    }
}