using KompasCore.Cards;
using UnityEngine;

namespace KompasServer.Effects
{
    public class DeckTargetSubeffect : CardTargetSubeffect
    {
        protected override void GetTargets()
        {
            base.GetTargets();
            EffectController.ServerNotifier.GetDeckTarget(this);
        }

        public override bool Resolve()
        {
            //check first that there exist valid targets. if there exist no valid targets, finish resolution here
            if (!ThisCard.Game.ExistsCardTarget(cardRestriction))
            {
                Debug.Log("No target exists for " + ThisCard.CardName + " effect");
                return ServerEffect.EffectImpossible();
            }


            //ask the client that is this effect's controller for a target. 
            //give the card if whose effect it is, the index of the effect, and the index of the subeffect
            //since only the server resolves effects, this should never be called for a client. 
            GetTargets();

            //then wait for the network controller to call the continue method
            return false;
        }

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