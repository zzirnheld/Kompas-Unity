namespace KompasCore.Effects.Identities.Stackables
{

    public class StackableIndex : ContextlessLeafIdentityBase<IStackable>
    {
        public int index = -1;

        protected override IStackable AbstractItem
            => EffectHelpers.GetItem(InitializationContext.effect.stackableTargets, index);
    }
}