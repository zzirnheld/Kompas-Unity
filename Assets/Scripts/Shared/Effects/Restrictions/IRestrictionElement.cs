
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
        protected static bool MultipleNonNull(params object[] objs) => objs.Count(o => o != null) > 1;

        public bool IsValid(Type item, IResolutionContext context)
        {
            ComplainIfNotInitialized();

            return item != null && IsValidLogic(item, context);
        }

        protected abstract bool IsValidLogic(Type item, IResolutionContext context);
    }
}