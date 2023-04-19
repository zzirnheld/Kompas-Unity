
using System.Linq;

namespace KompasCore.Effects
{
    public abstract class RestrictionElementBase<Type> : ContextInitializeableBase, IRestriction<Type>
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