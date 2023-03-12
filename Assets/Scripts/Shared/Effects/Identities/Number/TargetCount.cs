using KompasCore.Cards;
using System.Linq;

namespace KompasCore.Effects.Identities.Numbers
{

    public class TargetCount : ContextualParentIdentityBase<int>
    {
        public CardRestriction cardRestriction;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            cardRestriction?.Initialize(initializationContext);
        }

        private System.Func<GameCardBase, bool> Selector(ActivationContext context)
            => card => cardRestriction?.IsValidCard(card, InitializationContext.effect.CurrActivationContext) ?? true;

        protected override int AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
            => InitializationContext.subeffect.Effect.CardTargets.Count(Selector(context));
    }
}