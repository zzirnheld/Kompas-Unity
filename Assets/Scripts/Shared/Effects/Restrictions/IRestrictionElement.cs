
using System.Linq;

namespace KompasCore.Effects
{
    public abstract class RestrictionElementBase<Type> : ContextInitializeableBase, IRestriction<Type>
    {
        public bool IsValid(Type item, IResolutionContext context)
        {
            ComplainIfNotInitialized();

            return item != null && IsValidLogic(item, context);
        }

        protected abstract bool IsValidLogic(Type item, IResolutionContext context);
    }
}