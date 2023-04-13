
namespace KompasCore.Effects
{

    public interface IRestrictionElement<Type> : IContextInitializeable
    {
        public bool IsValid(Type item, IResolutionContext context);
    }

    public abstract class RestrictionElementBase<Type> : ContextInitializeableBase, IRestrictionElement<Type>
    {
        public bool IsValid(Type item, IResolutionContext context)
        {
            ComplainIfNotInitialized();

            return item != null && IsValidLogic(item, context);
        }

        protected abstract bool IsValidLogic(Type item, IResolutionContext context);
    }
}