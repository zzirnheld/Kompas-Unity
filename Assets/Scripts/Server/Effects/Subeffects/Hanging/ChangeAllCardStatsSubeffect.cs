using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace KompasServer.Effects.Subeffect.Hanging
{
    public class ChangeAllCardStatsSubeffect : ChangeCardStatsSubeffect
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
            cardRestriction.Initialize(DefaultInitializationContext);
        }

        protected override IEnumerable<HangingEffect> CreateHangingEffects()
        {
            var effs = new List<HangingEffect>();

            IEnumerable<GameCard> cards
                = ServerGame.Cards.Where(c => cardRestriction.IsValidCard(c, CurrentContext));

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
            var contextCopy = CurrentContext.Copy;
            contextCopy.SetResumeInfo(Effect.CardTargets, Effect.SpaceTargets, Effect.stackableTargets,
                CardTarget, SpaceTarget, StackableTarget);

            foreach (var card in cards)
            {
                var temp = new ChangeCardStats(game: ServerGame,
                                                 triggerRestriction: triggerRestriction,
                                                 endCondition: endCondition,
                                                 fallOffCondition: fallOffCondition,
                                                 fallOffRestriction: CreateFallOffRestriction(card),
                                                 sourceEff: Effect,
                                                 currentContext: contextCopy,
                                                 buffRecipient: card,
                                                 buff: buff);

                effs.Add(temp);
            }

            return effs;
        }
    }
}