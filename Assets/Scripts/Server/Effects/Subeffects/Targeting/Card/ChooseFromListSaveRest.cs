using KompasCore.Cards;
using KompasCore.Effects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
    public class ChooseFromListSaveRest : ChooseFromList
    {
        public CardRestriction restRestriction;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            if (restRestriction == null) restRestriction = cardRestriction;
            else restRestriction.Initialize(DefaultInitializationContext);
        }

        protected override Task<ResolutionInfo> NoPossibleTargets()
        {
            var rest = ServerGame.Cards.Where(c => restRestriction.IsValidCard(c, ResolutionContext));
            ServerEffect.rest.AddRange(rest);
            return base.NoPossibleTargets();
        }

        protected override void AddList(IEnumerable<GameCard> choices)
        {
            base.AddList(choices);
            var rest = ServerGame.Cards.Where(c => restRestriction.IsValidCard(c, ResolutionContext) && !choices.Contains(c));
            ServerEffect.rest.AddRange(rest);
        }
    }
}