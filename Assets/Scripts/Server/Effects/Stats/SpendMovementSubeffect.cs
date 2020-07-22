namespace KompasServer.Effects
{
    public class SpendMovementSubeffect : ServerSubeffect
    {
        public int multiplier = 0;
        public int modifier = 0;
        public int divisor = 1;

        public override bool Resolve()
        {
            var spaces = Effect.X * multiplier / divisor + modifier;
            if (Target.SpacesCanMove >= spaces)
            {
                Target.SpacesMoved += spaces;
                return ServerEffect.ResolveNextSubeffect();
            }
            else return ServerEffect.EffectImpossible();
        }
    }
}