using KompasCore.Cards;
using UnityEngine;

namespace KompasServer.Effects
{
    public class DeckTargetSubeffect : CardTargetSubeffect
    {

        public override bool AddTargetIfLegal(GameCard card)
        {
            //evaluate the target. if it's valid, confirm it as the target (that's what the true is for)
            if (AwaitingTarget && cardRestriction.Evaluate(card))
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