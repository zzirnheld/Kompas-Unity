using KompasCore.Cards;
using KompasCore.Effects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
    public class ChooseFromListSaveRestShared : ChooseFromList
    {
        //Restriction to specify what subset of cards fitting the cardRestriction should be sent as targets. Any among the possible choices will be saved as the rest
        public CardRestriction chooseRestriction;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);

            if (chooseRestriction == null) throw new System.ArgumentNullException("chooseRestriction");
            chooseRestriction.Initialize(DefaultInitializationContext);
        }

        protected override bool IsValidTarget(GameCard card) => base.IsValidTarget(card) && chooseRestriction.IsValid(card, ResolutionContext);

        protected override Task<ResolutionInfo> NoPossibleTargets()
        {
            var rest = ServerGame.Cards.Where(base.IsValidTarget);
            ServerEffect.rest.AddRange(rest);
            return base.NoPossibleTargets();
        }

        protected override void AddList(IEnumerable<GameCard> choices)
        {
            base.AddList(choices);
            var rest = ServerGame.Cards.Where(c => base.IsValidTarget(c) && !choices.Contains(c));
            ServerEffect.rest.AddRange(rest);
        }
    }
}