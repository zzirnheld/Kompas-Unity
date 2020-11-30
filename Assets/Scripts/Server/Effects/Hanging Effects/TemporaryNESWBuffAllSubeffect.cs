using KompasCore.Cards;
using KompasCore.Effects;
using System.Collections.Generic;
using System.Linq;

namespace KompasServer.Effects
{
    public class TemporaryNESWBuffAllSubeffect : TemporaryCardChangeSubeffect
    {
        public int nBuff;
        public int eBuff;
        public int sBuff;
        public int wBuff;

        //default to making sure things are characters before changing their stats
        public CardRestriction cardRestriction;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            cardRestriction = cardRestriction ?? new CardRestriction()
            {
                cardRestrictions = new string[]
                {
                    CardRestriction.IsCharacter
                }
            };
            cardRestriction.Initialize(this);
        }

        protected override IEnumerable<HangingEffect> CreateHangingEffects()
        {
            var effs = new List<HangingEffect>();

            IEnumerable<GameCard> cards = ServerGame.Cards.Where(c => cardRestriction.Evaluate(c));

            foreach (var card in cards)
            {
                var temp = new TemporaryNESWBuff(ServerGame, triggerRestriction, endCondition, 
                    fallOffCondition, CreateFallOffRestriction(card),
                    card, nBuff, eBuff, sBuff, wBuff);
            }

            return effs;
        }
    }
}