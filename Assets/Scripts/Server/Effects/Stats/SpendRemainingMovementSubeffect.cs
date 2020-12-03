namespace KompasServer.Effects
{
    public class SpendRemainingMovementSubeffect : ServerSubeffect
    {
        public int mult = 1;
        public int div = 1;
        public int mod = 0;

        public override bool Resolve()
        {
            int toSpend = (Target.SpacesCanMove * mult / div) + mod;
            int toSet = Target.SpacesMoved + toSpend;
            Target.SetSpacesMoved(toSet);
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}