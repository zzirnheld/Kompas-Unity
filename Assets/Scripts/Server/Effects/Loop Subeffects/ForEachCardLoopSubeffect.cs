using KompasCore.Cards;
using KompasCore.Effects.Identities;
using System.Collections.Generic;
using System.Linq;

namespace KompasServer.Effects
{
    public class ForEachCardLoopSubeffect : LoopSubeffect
    {
        public INoActivationContextIdentity<ICollection<GameCardBase>> cards;

        private List<GameCardBase> cardsToLoopOver;
        private GameCard lastTarget;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            cards.Initialize(DefaultRestrictionContext);
        }

        protected override void OnLoopEnter()
        {
            base.OnLoopEnter();
            cardsToLoopOver = cards.Item.ToList();
        }

        protected override void OnLoopExit()
        {
            base.OnLoopExit();
            cardsToLoopOver = default;
        }

        protected override bool ShouldContinueLoop
        {
            get
            {
                if (cardsToLoopOver.Count > 0)
                {
                    while (CardTarget != lastTarget) RemoveTarget();

                    lastTarget = cardsToLoopOver[0].Card;
                    cardsToLoopOver.RemoveAt(0);
                    Effect.AddTarget(lastTarget);
                    return true;
                }
                else return false;
            }
        }
    }
}