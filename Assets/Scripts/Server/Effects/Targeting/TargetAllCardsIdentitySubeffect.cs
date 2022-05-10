using KompasServer.Effects.Identities;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class TargetAllCardsIdentitySubeffect : ServerSubeffect
    {
        public SubeffectManyCardsIdentity cardsIdentity;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            cardsIdentity.Initialize(RestrictionContext);
        }

        public override Task<ResolutionInfo> Resolve()
        {
            var cards = cardsIdentity.Cards;
            if (cards.Count == 0) return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));

            foreach (var card in cards)
            {
                if (card == null) continue;
                Effect.AddTarget(card.Card);
            }

            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}