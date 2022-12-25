using KompasCore.Cards;
using KompasCore.Effects.Identities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class TargetAllCardsIdentitySubeffect : ServerSubeffect
    {
        public IActivationContextIdentity<IReadOnlyCollection<GameCardBase>> cardsIdentity;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            cardsIdentity.Initialize(DefaultInitializationContext);
        }

        public override Task<ResolutionInfo> Resolve()
        {
            var cards = cardsIdentity.From(CurrentContext, CurrentContext);
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