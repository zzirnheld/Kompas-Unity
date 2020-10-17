using KompasCore.Cards;
using UnityEngine;


namespace KompasServer.Effects
{
    [System.Serializable]
    public class BoardTargetSubeffect : CardTargetSubeffect
    {
        protected override void GetTargets()
        {
            base.GetTargets();
            EffectController.ServerNotifier.GetBoardTarget(this);
        }

        public override bool Resolve()
        {
            //check first that there exist valid targets. if there exist no valid targets, finish resolution here
            if (!ThisCard.Game.ExistsCardTarget(cardRestriction))
            {
                Debug.Log("No target exists for " + ThisCard.CardName + " effect");
                ServerEffect.EffectImpossible();
                return false;
            }

            //ask the client that is this effect's controller for a target. 
            //give the card if whose effect it is, the index of the effect, and the index of the subeffect
            //since only the server resolves effects, this should never be called for a client.
            GetTargets();

            //then wait for the network controller to call the continue method
            return false;
        }
    }
}