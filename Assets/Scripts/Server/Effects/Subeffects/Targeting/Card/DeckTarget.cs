using KompasCore.Cards;
using KompasCore.Effects.Identities;
using KompasCore.GameCore;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasServer.Effects.Subeffects
{
    /// <summary>
    /// Gets a target from the effect controller's deck. Shuffles the deck aftewards
    /// </summary>
    public class DeckTargetSubeffect : CardTarget
    {
        protected override IEnumerable<GameCard> TargetCardsSource => Controller.deckCtrl.Cards;

        public override bool AddTargetIfLegal(GameCard card)
        {
            //evaluate the target. if it's valid, confirm it as the target (that's what the true is for)
            if (cardRestriction.IsValidCard(card, CurrentContext))
            {
                card.Controller.deckCtrl.Shuffle();
                ServerEffect.AddTarget(card);
                ServerPlayer.notifier.AcceptTarget();
                return true;
            }
            else
            {
                Debug.Log($"{card?.CardName} not a valid target for {cardRestriction}");
                return false;
            }
        }
    }

    public class PartialDeckTargetSubeffect : CardTarget
    {
        public IIdentity<int> numberOfTopCards;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            numberOfTopCards.Initialize(DefaultInitializationContext);
        }

        protected override IEnumerable<GameCard> TargetCardsSource
            => Controller.deckCtrl.Cards.Take(numberOfTopCards.From(CurrentContext, default));

        public override bool AddTargetIfLegal(GameCard card)
        {
            //evaluate the target. if it's valid, confirm it as the target (that's what the true is for)
            if (cardRestriction.IsValidCard(card, CurrentContext))
            {
                DeckController.BottomdeckMany(TargetCardsSource.Where(c => c != card));
                ServerEffect.AddTarget(card);
                ServerPlayer.notifier.AcceptTarget();
                return true;
            }
            else
            {
                Debug.Log($"{card?.CardName} not a valid target for {cardRestriction}");
                return false;
            }
        }
    }
}