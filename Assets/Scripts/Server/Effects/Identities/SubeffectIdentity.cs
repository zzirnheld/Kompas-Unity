using KompasCore.Effects;
using KompasCore.Effects.Identities;

namespace KompasServer.Effects.Identities
{
    public abstract class SubeffectIdentityBase<ReturnType> : SubeffectInitializeableBase,
        INoActivationContextIdentity<ReturnType>
    {
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