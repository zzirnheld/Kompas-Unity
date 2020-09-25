namespace KompasServer.Effects
{
    [System.Serializable]
    public class XChangeNESWSubeffect : ServerSubeffect
    {
        public int nMult = 0;
        public int eMult = 0;
        public int sMult = 0;
        public int wMult = 0;

        public int nMod = 0;
        public int eMod = 0;
        public int sMod = 0;
        public int wMod = 0;

        public int NVal => ServerEffect.X * nMult + nMod;
        public int EVal => ServerEffect.X * eMult + eMod;
        public int SVal => ServerEffect.X * sMult + sMod;
        public int WVal => ServerEffect.X * wMult + wMod;

        public override bool Resolve()
        {
            Target.AddToCharStats(NVal, EVal, SVal, WVal);
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}