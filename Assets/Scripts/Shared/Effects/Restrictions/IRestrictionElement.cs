
using System.Linq;

namespace KompasCore.Effects
{

    public interface IRestrictionElement<Type> : IContextInitializeable
    {
        public bool IsValid(Type item, IResolutionContext context);
    }

    public abstract class RestrictionElementBase<Type> : ContextInitializeableBase, IRestrictionElement<Type>
    {
        protected static bool AllNull(params object[] objs) => objs.All(o => o == null);

        public bool IsValid(Type item, IResolutionContext context)
        {
            ComplainIfNotInitialized();

            return item != null && IsValidLogic(item, context);
        }

        protected abstract bool IsValidLogic(Type item, IResolutionContext context);
    }
}