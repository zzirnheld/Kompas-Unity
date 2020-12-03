using KompasCore.Cards;
using KompasCore.Effects;
using System.Linq;
using UnityEngine;

namespace KompasServer.Effects
{
    public class CardTargetSubeffect : ServerSubeffect
    {
        public CardRestriction cardRestriction;

        public bool AwaitingTarget { get; protected set; }

        protected virtual int[] PotentialTargetIds
            => Game.Cards.Where(c => cardRestriction.Evaluate(c)).Select(c => c.ID).ToArray();

        protected virtual void GetTargets(int[] potentialTargetIds)
        {
            AwaitingTarget = true;
            Debug.Log($"Asking for card target among ids {string.Join(", ", potentialTargetIds)}");
            EffectController.ServerNotifier.GetCardTarget(Source.CardName, cardRestriction.blurb, potentialTargetIds, null);
        }

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            cardRestriction = cardRestriction ?? new CardRestriction();
            cardRestriction.Initialize(this);
        }

        public override bool Resolve()
        {
            var potentialTargetIds = PotentialTargetIds;

            //check first that there exist valid targets. if there exist no valid targets, finish resolution here
            if (!potentialTargetIds.Any())
            {
                Debug.Log($"No target exists for {ThisCard.CardName} effect");
                return ServerEffect.EffectImpossible();
            }

            //ask the client that is this effect's controller for a target. 
            //give the card if whose effect it is, the index of the effect, and the index of the subeffect
            //since only the server resolves effects, this should never be called for a client. 
            GetTargets(potentialTargetIds);

            //then wait for the network controller to call the continue method
            return false;
        }

        /// <summary>
        /// Check if the card passed is a valid target, and if it is, continue the effect
        /// </summary>
        public virtual bool AddTargetIfLegal(GameCard card)
        {
            //don't do anyting if we're not waiting for a target selection.
            if (!AwaitingTarget) return false;
            //evaluate the target. if it's valid, confirm it as the target (that's what the true is for)
            else if (cardRestriction.Evaluate(card))
            {
                AwaitingTarget = false;
                ServerEffect.AddTarget(card);
                EffectController.ServerNotifier.AcceptTarget();
                return ServerEffect.ResolveNextSubeffect();
            }
            else
            {
                GetTargets(PotentialTargetIds);
                return false;
            }
        }
    }
}