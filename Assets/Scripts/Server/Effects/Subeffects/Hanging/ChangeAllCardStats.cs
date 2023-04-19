using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Effects.Restrictions.CardRestrictionElements;
using KompasCore.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace KompasServer.Effects.Subeffects.Hanging
{
    public class ChangeAllCardStats : ChangeCardStats
    {
        //default to making sure things are characters before changing their stats
        public CardRestriction cardRestriction;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            cardRestriction ??= new CardRestriction()
            {
                elements = new IRestriction<GameCardBase>[] { new Character() }
            };
            cardRestriction.Initialize(DefaultInitializationContext);
        }

        protected override IEnumerable<HangingEffect> CreateHangingEffects()
        {
            var effs = new List<HangingEffect>();

            IEnumerable<GameCard> cards
                = ServerGame.Cards.Where(c => cardRestriction.IsValid(c, ResolutionContext));

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
                var temp = new ChangeCardStatsEffect(game: ServerGame,
                                                    triggerRestriction: triggerRestriction,
                                                    endCondition: endCondition,
                                                    fallOffCondition: fallOffCondition,
                                                    fallOffRestriction: CreateFallOffRestriction(card),
                                                    sourceEff: Effect,
                                                    currentContext: ResolutionContext,
                                                    buffRecipient: card,
                                                    buff: buff);

                effs.Add(temp);
            }

            return effs;
        }
    }
}