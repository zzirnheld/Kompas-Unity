using KompasCore.Effects;
using KompasCore.Effects.Identities;

namespace KompasServer.Effects.Identities
{

    public abstract class SubeffectIdentityBase<ReturnType> : SubeffectInitializeableBase,
        INoActivationContextIdentity<ReturnType>
    {
        public bool secondary = false;

        /// <summary>
        /// Override this one if you ONLY need to know about the context you should actually be considering
        /// </summary>
        /// <param name="contextToConsider">The ActivationContext you actually should be considering.</param>
        protected abstract ReturnType AbstractItem { get; }

        /// <summary>
        /// Gets the abstract stackable from the first one, that only knows about the context to consider,
        /// then the one that knows about both contexts if the first one came up empty.
        /// </summary>
        public ReturnType Item
        {
            get
            {
                ComplainIfNotInitialized();

                return AbstractItem;
            }
        }
    }
}