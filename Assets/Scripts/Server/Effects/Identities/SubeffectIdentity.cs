using KompasCore.Effects;
using KompasCore.Effects.Identities;

namespace KompasServer.Effects.Identities
{

    public abstract class SubeffectIdentityBase<ReturnType> : SubeffectInitializeableBase,
        INoActivationContextIdentity<ReturnType>, IActivationContextIdentity<ReturnType>
    {
        public bool secondary = false;

        protected abstract ReturnType AbstractItem { get; }

        public ReturnType Item
        {
            get
            {
                ComplainIfNotInitialized();

                return AbstractItem;
            }
        }

        //Ignores the given contexts, because it'll just use the current context for the subeffect.
        //Use an explicit ActivationContextIdentity if you want it to not ignore the context,
        //using the FromContext subeffect identity
        public ReturnType From(ActivationContext context, ActivationContext secondaryContext) => Item;
    }
}