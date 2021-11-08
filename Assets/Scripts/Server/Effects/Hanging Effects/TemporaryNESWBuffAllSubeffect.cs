using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Exceptions;
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
            cardRestriction ??= new CardRestriction()
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

            IEnumerable<GameCard> cards 
                = ServerGame.Cards.Where(c => cardRestriction.Evaluate(c, Context));

            //First make sure are allowed to set their stats. 
            //Testing here so later changes can maybe be allowed to allow this to occur,
            //and so test each to be affected before any are affected
            foreach (var card in cards)
            {
                if (card.Location != CardLocation.Field)
                    throw new InvalidLocationException(card.Location, card, ChangedStatsOfCardOffBoard);
            }

            foreach (var card in cards)
            {
                var temp = new TemporaryNESWBuff(game: ServerGame,
                                                 triggerRestriction: triggerRestriction,
                                                 endCondition: endCondition,
                                                 fallOffCondition: fallOffCondition,
                                                 fallOffRestriction: CreateFallOffRestriction(card),
                                                 sourceEff: Effect,
                                                 currentContext: Context,
                                                 buffRecipient: card,
                                                 nBuff: nBuff,
                                                 eBuff: eBuff,
                                                 sBuff: sBuff,
                                                 wBuff: wBuff);
            }

            return effs;
        }
    }
}