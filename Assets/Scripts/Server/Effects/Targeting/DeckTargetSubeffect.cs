using KompasCore.Cards;
using UnityEngine;

namespace KompasServer.Effects
{
    public class DeckTargetSubeffect : CardTargetSubeffect
    {

        public override bool AddTargetIfLegal(GameCard card)
        {
            if (!AwaitingTarget) return false;
            //evaluate the target. if it's valid, confirm it as the target (that's what the true is for)
            else if (cardRestriction.Evaluate(card))
            {
                AwaitingTarget = false;
                Debug.Log("Adding " + card.CardName + " as target");
                ServerEffect.AddTarget(card);
                EffectController.ServerNotifier.AcceptTarget();
                card.Controller.deckCtrl.Shuffle();
                return ServerEffect.ResolveNextSubeffect();
            }
            else GetTargets();

            return false;
        }
    }
}