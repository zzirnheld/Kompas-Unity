namespace KompasServer.Effects
{
    public class SetCardStatsSubeffect : ServerSubeffect
    {
        public int nVal = -1;
        public int eVal = -1;
        public int sVal = -1;
        public int wVal = -1;
        public int cVal = -1;
        public int aVal = -1;

        public int RealNVal => nVal < 0 ? Target.N : nVal;
        public int RealEVal => eVal < 0 ? Target.E : eVal;
        public int RealSVal => sVal < 0 ? Target.S : sVal;
        public int RealWVal => wVal < 0 ? Target.W : wVal;
        public int RealCVal => cVal < 0 ? Target.C : cVal;
        public int RealAVal => aVal < 0 ? Target.A : aVal;

        public override bool Resolve()
        {
            Target.SetStats((RealNVal, RealEVal, RealSVal, RealWVal, RealCVal, RealAVal));
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}