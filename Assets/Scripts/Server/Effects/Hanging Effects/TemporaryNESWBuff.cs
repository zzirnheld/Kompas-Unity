using KompasCore.Cards;
using KompasCore.Effects;
using KompasServer.GameCore;

namespace KompasServer.Effects
{
    public class TemporaryNESWBuff : HangingEffect
    {
        private readonly GameCard buffRecipient;
        private readonly int nBuff = 0;
        private readonly int eBuff = 0;
        private readonly int sBuff = 0;
        private readonly int wBuff = 0;
        private readonly int cBuff = 0;
        private readonly int aBuff = 0;

        public TemporaryNESWBuff(ServerGame game, TriggerRestriction triggerRestriction, string endCondition, 
            string fallOffCondition, TriggerRestriction fallOffRestriction, Effect sourceEff,
            ActivationContext currentContext, GameCard buffRecipient, int nBuff, int eBuff, int sBuff, int wBuff, int cBuff, int aBuff)
            : base(game, triggerRestriction, endCondition, fallOffCondition, fallOffRestriction, sourceEff, currentContext, removeIfEnd: true)
        {
            this.buffRecipient = buffRecipient != null ? buffRecipient : throw new System.ArgumentNullException("buffRecipient", "Null characcter card in temporary nesw buff");
            this.nBuff = nBuff;
            this.eBuff = eBuff;
            this.sBuff = sBuff;
            this.wBuff = wBuff;
            this.cBuff = cBuff;
            this.aBuff = aBuff;

            buffRecipient.AddToStats((nBuff, eBuff, sBuff, wBuff, cBuff, aBuff), stackSrc: sourceEff);
        }

        protected override void Resolve(ActivationContext context)
            => buffRecipient.AddToStats((-1 * nBuff, -1 * eBuff, -1 * sBuff, -1 * wBuff, -1 * cBuff, -1 * aBuff), stackSrc: sourceEff);
    }
}