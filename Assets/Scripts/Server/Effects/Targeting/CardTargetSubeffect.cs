using KompasCore.Cards;
using KompasCore.Effects;
using UnityEngine;

namespace KompasServer.Effects
{
    public abstract class CardTargetSubeffect : ServerSubeffect
    {
        public CardRestriction cardRestriction = new CardRestriction();

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            cardRestriction.Initialize(this);
        }

        /// <summary>
        /// Check if the card passed is a valid target, and if it is, continue the effect
        /// </summary>
        public virtual bool AddTargetIfLegal(GameCard card)
        {
            //evaluate the target. if it's valid, confirm it as the target (that's what the true is for)
            if (cardRestriction.Evaluate(card))
            {
                Debug.Log("Adding " + card.CardName + " as target");
                ServerEffect.AddTarget(card);
                EffectController.ServerNotifier.AcceptTarget();
                ServerEffect.ResolveNextSubeffect();
                return true;
            }

            return false;
        }
    }
}