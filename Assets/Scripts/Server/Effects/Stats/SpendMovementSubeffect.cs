namespace KompasServer.Effects
{
    public class SpendMovementSubeffect : ServerSubeffect
    {
        public override bool Resolve()
        {
            var spaces = Count;
            if (Target.SpacesCanMove >= spaces)
            {
                Target.SpacesMoved += spaces;
                return ServerEffect.ResolveNextSubeffect();
            }
            else return ServerEffect.EffectImpossible();
        }
    }
}