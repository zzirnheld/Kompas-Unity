using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace KompasServer.Effects
{
    public class TemporaryCardStatChangeAllSubeffect : TemporaryCardStatChangeSubeffect
    {
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
            //Don't affect any card unless all that should be affected, can be.
            foreach (var card in cards)
            {
                if (card == null)
                    throw new NullCardException(TargetWasNull);
                else if (forbidNotBoard && card.Location != CardLocation.Board)
                    throw new InvalidLocationException(card.Location, card, ChangedStatsOfCardOffBoard);
            }

            var buff = Buff;

            foreach (var card in cards)
            {
                var temp = new TemporaryCardStatChange(game: ServerGame,
                                                 triggerRestriction: triggerRestriction,
                                                 endCondition: endCondition,
                                                 fallOffCondition: fallOffCondition,
                                                 fallOffRestriction: CreateFallOffRestriction(card),
                                                 sourceEff: Effect,
                                                 currentContext: Context,
                                                 buffRecipient: card,
                                                 buff: buff);
            }

            return effs;
        }
    }
}