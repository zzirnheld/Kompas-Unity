using System.Linq;

namespace KompasCore.Effects.Restrictions
{
    public class StackableRestriction : ContextInitializeableBase
    {
        public IStackableRestrictionElement[] restrictionElements = { };

        public bool Evaluate(IStackable stackable)
        {
            ComplainIfNotInitialized();
            return restrictionElements.All(re => re.IsValidStackable(stackable));
        }
    }

    public interface IStackableRestrictionElement
    {
        public bool IsValidStackable(IStackable stackable);
    }

    public abstract class StackableRestrictionElementBase : ContextInitializeableBase,
        IStackableRestrictionElement
    {
        public bool IsValidStackable(IStackable stackable)
        {
            ComplainIfNotInitialized();
            return AbstractIsValidStackable(stackable);
        }

        protected abstract bool AbstractIsValidStackable(IStackable stackable);
    }
}