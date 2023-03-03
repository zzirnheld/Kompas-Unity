using KompasCore.Exceptions;

namespace KompasCore.Effects.Identities
{
    public interface IActivationContextIdentity<ReturnType> : IContextInitializeable
    {
        public ReturnType From(ActivationContext context, ActivationContext secondaryContext);
    }

    public abstract class ActivationContextIdentityBase<ReturnType> : ContextInitializeableBase,
        IActivationContextIdentity<ReturnType>
    {
        public bool secondary = false;

        /// <summary>
        /// Override this one if you ONLY need to know about the context you should actually be considering
        /// </summary>
        /// <param name="contextToConsider">The ActivationContext you actually should be considering.</param>
        protected virtual ReturnType AbstractItemFrom(ActivationContext contextToConsider) => default;

        /// <summary>
        /// Override this one if you need to pass on BOTH contexts.
        /// </summary>
        protected virtual ReturnType AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext) => default;

        /// <summary>
        /// Gets the abstract stackable from the first one, that only knows about the context to consider,
        /// then the one that knows about both contexts if the first one came up empty.
        /// </summary>
        public ReturnType From(ActivationContext context, ActivationContext secondaryContext)
        {
            ComplainIfNotInitialized();

            ActivationContext contextToConsider = toConsider(context, secondaryContext);

            return AbstractItemFrom(contextToConsider)
                ?? AbstractItemFrom(context, secondaryContext)
                ?? throw new NullCardException($"Neither identity of the context identity returned non-null...");
        }

        protected ActivationContext toConsider(ActivationContext context, ActivationContext secondaryContext)
            => secondary ? secondaryContext : context;
    }
}