﻿using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class ChangeCardStatsSubeffect : ServerSubeffect
    {
        public int nMult = 0;
        public int eMult = 0;
        public int sMult = 0;
        public int wMult = 0;
        public int cMult = 0;
        public int aMult = 0;

        public int nDiv = 1;
        public int eDiv = 1;
        public int sDiv = 1;
        public int wDiv = 1;
        public int cDiv = 1;
        public int aDiv = 1;

        public int nMod = 0;
        public int eMod = 0;
        public int sMod = 0;
        public int wMod = 0;
        public int cMod = 0;
        public int aMod = 0;

        public int NVal => ServerEffect.X * nMult / nDiv  + nMod;
        public int EVal => ServerEffect.X * eMult / eDiv + eMod;
        public int SVal => ServerEffect.X * sMult / sDiv + sMod;
        public int WVal => ServerEffect.X * wMult / wDiv + wMod;
        public int CVal => ServerEffect.X * cMult / cDiv + cMod;
        public int AVal => ServerEffect.X * aMult / aDiv + aMod;
        public (int, int, int, int, int, int) StatValues => (NVal, EVal, SVal, WVal, CVal, AVal);

        public override Task<ResolutionInfo> Resolve()
        {
            if (Target == null)
                throw new NullCardException(TargetWasNull);
            else if (forbidNotBoard && Target.Location != CardLocation.Field)
                throw new InvalidLocationException(Target.Location, Target, ChangedStatsOfCardOffBoard);

            Target.AddToStats(StatValues, Effect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}