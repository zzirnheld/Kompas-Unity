using KompasCore.Exceptions;

namespace KompasCore.Effects.Identities
{
    /// <summary>
    /// Can be used whether or not the caller does or doesn't care about an ActivationContext.
    /// (Eventually, can be used whether or not the caller does or doesn't care about an ActivationContext,
    /// but that interface doesn't even exist yet.)
    /// </summary>
    public interface IIdentity<ReturnType> : IContextInitializeable
    {
        public ReturnType From(ActivationContext context, ActivationContext secondaryContext);
    }

    /// <summary>
    /// An identity that needs context, either for itself or to pass on to its children.
    /// </summary>
    public abstract class ContextualParentIdentityBase<ReturnType> : ContextInitializeableBase,
        IIdentity<ReturnType>
    {
        public bool secondary = false;

        /// <summary>
        /// Override this one if you need to pass on BOTH contexts.
        /// </summary>
        protected abstract ReturnType AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext);

        /// <summary>
        /// Gets the abstract stackable from the first one, that only knows about the context to consider,
        /// then the one that knows about both contexts if the first one came up empty.
        /// </summary>
        public ReturnType From(ActivationContext context, ActivationContext secondaryContext)
        {
            ComplainIfNotInitialized();

            return AbstractItemFrom(context, secondaryContext);
        }

        protected ActivationContext toConsider(ActivationContext context, ActivationContext secondaryContext)
            => secondary ? secondaryContext : context;

        public ReturnType Item => From(InitializationContext.effect.CurrActivationContext, default);

        protected Attack GetAttack(ActivationContext context)
        {
            if (context.stackableEvent is Attack eventAttack) return eventAttack;
            if (context.stackableCause is Attack causeAttack) return causeAttack;
            else throw new NullCardException("Stackable event wasn't an attack!");
        }
    }

    public abstract class ContextualLeafIdentityBase<ReturnType> : ContextualParentIdentityBase<ReturnType>
    {
        /// <summary>
        /// Override this one if you ONLY need to know about the context you should actually be considering
        /// </summary>
        /// <param name="contextToConsider">The ActivationContext you actually should be considering.</param>
        protected abstract ReturnType AbstractItemFrom(ActivationContext contextToConsider);

        /// <summary>
        /// Gets the abstract stackable from the first one, that only knows about the context to consider,
        /// then the one that knows about both contexts if the first one came up empty.
        /// </summary>
        protected override ReturnType AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
        {
            ActivationContext contextToConsider = toConsider(context, secondaryContext);
            return AbstractItemFrom(contextToConsider);
        }
    }

    public abstract class ContextlessLeafIdentityBase<ReturnType> : ContextInitializeableBase,
        IIdentity<ReturnType>
    {
        protected abstract ReturnType AbstractItem { get; }

        public ReturnType Item
        {
            get
            {
                ComplainIfNotInitialized();
                return AbstractItem;
            }
        }

        public ReturnType From(ActivationContext context, ActivationContext secondaryContext) => Item;
    }
}