using System.Collections.Generic;
using System.Linq;
using KompasCore.Cards;
using KompasCore.Effects;

namespace KompasServer.Effects
{
    public class ChooseFromListSubeffect : ServerSubeffect
    {
        /// <summary>
        /// Restriction that each card must fulfill
        /// </summary>
        public CardRestriction cardRestriction = new CardRestriction();

        /// <summary>
        /// Restriction that the list collectively must fulfill
        /// </summary>
        public ListRestriction listRestriction = new ListRestriction();

        /// <summary>
        /// The maximum number of cards that can be chosen.
        /// If this isn't specified in the json, allow unlimited cards to be chosen.
        /// Represent this with -1
        /// </summary>
        public int maxCanChoose = -1;

        protected IEnumerable<GameCard> potentialTargets;

        protected void RequestTargets()
        {
            ServerPlayer.ServerNotifier.GetChoicesFromList(potentialTargets, maxCanChoose, this);
        }

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            cardRestriction.Initialize(this);
            listRestriction.Subeffect = this;
        }

        public override bool Resolve()
        {
            //TODO: somehow figure out a better way of checking if there exists a valid list?
            //  maybe a method on list restriction that checks?
            //  because otherwise enumerating lists and seeing if at least one fits would be exponential time
            if (!listRestriction.Evaluate(new List<GameCard>()))
                return ServerEffect.EffectImpossible();

            potentialTargets = ServerGame.Cards.Where(c => cardRestriction.Evaluate(c));

            //if there are no possible targets, declare the effect impossible
            //if you want to continue resolution anyway, add an if impossible check before this subeffect.
            if (potentialTargets.Any())
            {
                RequestTargets();
                return false;
            }
            else return ServerEffect.EffectImpossible();
        }

        public virtual bool AddListIfLegal(IEnumerable<GameCard> choices)
        {
            //check that there are no elements in choices that aren't in potential targets
            //also check that, if a maximum number to choose has been specified, that many have been chosen
            //also check that the list as a whole is allowable
            bool invalidList = (maxCanChoose > 0 && choices.Count() > maxCanChoose) ||
                choices.Intersect(potentialTargets).Count() != choices.Count() ||
                !listRestriction.Evaluate(choices);

            if (invalidList)
            {
                RequestTargets();
                return false;
            }

            //add all cards in the chosen list to targets
            foreach (var c in choices) ServerEffect.AddTarget(c);
            //everything's cool
            EffectController.ServerNotifier.AcceptTarget();
            ServerEffect.ResolveNextSubeffect();
            return true;
        }

    }
}