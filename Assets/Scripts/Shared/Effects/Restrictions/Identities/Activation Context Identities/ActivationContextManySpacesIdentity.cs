using KompasCore.Effects;
using KompasCore.Effects.Identities;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities
{
    public abstract class ActivationContextManySpacesIdentityBase : ContextInitializeableBase, 
        IActivationContextIdentity<ICollection<Space>>
    {
        protected abstract ICollection<Space> AbstractSpacesFrom(ActivationContext context);

        public ICollection<Space> From(ActivationContext context)
        {
            ComplainIfNotInitialized();
            return AbstractSpacesFrom(context);
        }
    }

    namespace ActivationContextManySpacesIdentities
    {
        /// <summary>
        /// Spaces where they are in some defined relationship with respect to the other two defined spaces.
        /// For example, spaces that are between (relationship) the source card's space and the target space (two defined spaces).
        /// </summary>
        public class ThreeSpaceRelationship : ActivationContextManySpacesIdentityBase
        {
            public IActivationContextIdentity<Space> firstSpace;
            public IActivationContextIdentity<Space> secondSpace;

            public IThreeSpaceRelationship thirdSpaceRelationship;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                firstSpace.Initialize(initializationContext);
                secondSpace.Initialize(initializationContext);
            }

            protected override ICollection<Space> AbstractSpacesFrom(ActivationContext context)
            {
                Space first = firstSpace.From(context);
                Space second = secondSpace.From(context);
                return Space.Spaces.Where(space => thirdSpaceRelationship.Evaluate(first, second, space)).ToArray();
            }
        }
    }
}