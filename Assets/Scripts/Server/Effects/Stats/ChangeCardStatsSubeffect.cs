namespace KompasServer.Effects
{
    [System.Serializable]
    public class ChangeCardStatsSubeffect : ServerSubeffect
    {
        public int nMult = 0;
        public int eMult = 0;
        public int sMult = 0;
        public int wMult = 0;
        public int cMult = 0;
        public int aMult = 0;

        public int nMod = 0;
        public int eMod = 0;
        public int sMod = 0;
        public int wMod = 0;
        public int cMod = 0;
        public int aMod = 0;

        public int NVal => ServerEffect.X * nMult + nMod;
        public int EVal => ServerEffect.X * eMult + eMod;
        public int SVal => ServerEffect.X * sMult + sMod;
        public int WVal => ServerEffect.X * wMult + wMod;
        public int CVal => ServerEffect.X * cMult + cMod;
        public int AVal => ServerEffect.X * aMult + aMod;
        public (int, int, int, int, int, int) StatValues => (NVal, EVal, SVal, WVal, CVal, AVal);

        public override bool Resolve()
        {
            Target.AddToStats(StatValues, Effect);
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}