using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities.ManySpaces
{
    public class Multiple : ContextualParentIdentityBase<IReadOnlyCollection<Space>>
    {
        public IIdentity<Space>[] spaces;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            foreach (var i in spaces) i.Initialize(initializationContext);
        }

        protected override IReadOnlyCollection<Space> AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
            => spaces.Select(s => s.From(context, secondaryContext)).ToArray();
    }
}