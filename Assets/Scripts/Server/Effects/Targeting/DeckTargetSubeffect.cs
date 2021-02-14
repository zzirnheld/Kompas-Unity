using KompasCore.Cards;
using UnityEngine;

namespace KompasServer.Effects
{
    public class DeckTargetSubeffect : CardTargetSubeffect
    {

        public override bool AddTargetIfLegal(GameCard card)
        {
            //evaluate the target. if it's valid, confirm it as the target (that's what the true is for)
            if (cardRestriction.Evaluate(card))
            {
                card.Controller.deckCtrl.Shuffle();
                ServerEffect.AddTarget(card);
                ServerPlayer.ServerNotifier.AcceptTarget();
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