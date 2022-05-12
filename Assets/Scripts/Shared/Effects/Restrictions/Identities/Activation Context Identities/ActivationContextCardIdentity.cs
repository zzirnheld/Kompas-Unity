using KompasCore.Cards;

namespace KompasCore.Effects.Identities
{
    namespace ActivationContextCardIdentities
    {

        public class MainCardBefore : ActivationContextIdentityBase<GameCardBase>
        {
            protected override GameCardBase AbstractItemFrom(ActivationContext context)
                => context.mainCardInfoBefore;
        }

        public class CardAtPosition : ActivationContextIdentityBase<GameCardBase>
        {
            public IActivationContextIdentity<Space> position;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                position.Initialize(initializationContext);
            }

            protected override GameCardBase AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
            {
                var finalSpace = position.From(context, secondaryContext);
                return context.game.boardCtrl.GetCardAt(finalSpace);
            }
        }

        public class TargetIndex : ActivationContextIdentityBase<GameCardBase>
        {
            public int index = -1;

            protected override GameCardBase AbstractItemFrom(ActivationContext contextToConsider)
                => Effect.GetItem(contextToConsider.CardTargets, index);
        }
    }
}