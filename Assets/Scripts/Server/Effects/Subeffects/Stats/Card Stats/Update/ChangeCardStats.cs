using KompasServer.Effects.Identities.SubeffectNumberIdentities;

namespace KompasServer.Effects.Subeffects
{
    public class ChangeCardStats : UpdateCardStats
    {
        public int nModifier = 0;
        public int eModifier = 0;
        public int sModifier = 0;
        public int wModifier = 0;
        public int cModifier = 0;
        public int aModifier = 0;

        public int nDivisor = 1;
        public int eDivisor = 1;
        public int sDivisor = 1;
        public int wDivisor = 1;
        public int cDivisor = 1;
        public int aDivisor = 1;

        public int nMultiplier = 0;
        public int eMultiplier = 0;
        public int sMultiplier = 0;
        public int wMultiplier = 0;
        public int cMultiplier = 0;
        public int aMultiplier = 0;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            nChange ??= new X() { multiplier = nMultiplier, modifier = nModifier, divisor = nDivisor };
            eChange ??= new X() { multiplier = eMultiplier, modifier = eModifier, divisor = eDivisor };
            sChange ??= new X() { multiplier = sMultiplier, modifier = sModifier, divisor = sDivisor };
            wChange ??= new X() { multiplier = wMultiplier, modifier = wModifier, divisor = wDivisor };
            cChange ??= new X() { multiplier = cMultiplier, modifier = cModifier, divisor = cDivisor };
            aChange ??= new X() { multiplier = aMultiplier, modifier = aModifier, divisor = aDivisor };

            base.Initialize(eff, subeffIndex);
        }
    }
}