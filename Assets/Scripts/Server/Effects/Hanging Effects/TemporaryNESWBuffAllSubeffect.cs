using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace KompasServer.Effects
{
    public class TemporaryNESWBuffAllSubeffect : HangingEffectSubeffect
    {
        public int nModifier = 0;
        public int eModifier = 0;
        public int sModifier = 0;
        public int wModifier = 0;
        public int cModifier = 0;
        public int aModifier = 0;

        public int nMultiplier = 0;
        public int eMultiplier = 0;
        public int sMultiplier = 0;
        public int wMultiplier = 0;
        public int cMultiplier = 0;
        public int aMultiplier = 0;

        //default to making sure things are characters before changing their stats
        public CardRestriction cardRestriction;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            cardRestriction ??= new CardRestriction()
            {
                cardRestrictions = new string[]
                {
                    CardRestriction.Character
                }
            };
            cardRestriction.Initialize(this);
        }

        protected override IEnumerable<HangingEffect> CreateHangingEffects()
        {
            var effs = new List<HangingEffect>();

            IEnumerable<GameCard> cards 
                = ServerGame.Cards.Where(c => cardRestriction.IsValidCard(c, Context));

            //First make sure are allowed to set their stats. 
            //Testing here so later changes can maybe be allowed to allow this to occur,
            //and so test each to be affected before any are affected
            foreach (var card in cards)
            {
                if (card == null)
                    throw new NullCardException(TargetWasNull);
                else if (forbidNotBoard && card.Location != CardLocation.Board)
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
                                                 nBuff: nModifier + Effect.X * nMultiplier,
                                                 eBuff: eModifier + Effect.X * eMultiplier,
                                                 sBuff: sModifier + Effect.X * sMultiplier,
                                                 wBuff: wModifier + Effect.X * wMultiplier,
                                                 cBuff: cModifier + Effect.X * cMultiplier,
                                                 aBuff: aModifier + Effect.X * aMultiplier);
            }

            return effs;
        }
    }
}