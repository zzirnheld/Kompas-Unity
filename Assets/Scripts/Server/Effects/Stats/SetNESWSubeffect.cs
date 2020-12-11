namespace KompasServer.Effects
{
    public class SetNESWSubeffect : ServerSubeffect
    {
        public int nVal = -1;
        public int eVal = -1;
        public int sVal = -1;
        public int wVal = -1;

        public int RealNVal => nVal < 0 ? Target.N : nVal;
        public int RealEVal => eVal < 0 ? Target.E : eVal;
        public int RealSVal => sVal < 0 ? Target.S : sVal;
        public int RealWVal => wVal < 0 ? Target.W : wVal;

        public override bool Resolve()
        {
            Target.SetCharStats(RealNVal, RealEVal, RealSVal, RealWVal, Effect);
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}