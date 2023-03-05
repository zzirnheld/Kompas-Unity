namespace KompasCore.Effects.Identities
{
    namespace ActivationContextStackableIdentities
    {
        public class StackableCause : ContextualLeafIdentityBase<IStackable>
        {
            protected override IStackable AbstractItemFrom(ActivationContext contextToConsider)
                => contextToConsider.stackableCause;
        }

        public class StackableIndex : ContextlessLeafIdentityBase<IStackable>
        {
            public int index = -1;

            protected override IStackable AbstractItem
                => EffectHelpers.GetItem(InitializationContext.effect.stackableTargets, index);
        }
    }
}