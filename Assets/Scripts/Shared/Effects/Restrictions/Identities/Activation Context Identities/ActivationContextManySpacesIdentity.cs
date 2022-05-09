using KompasCore.Effects;
using KompasCore.Effects.Identities;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities
{
    public interface IActivationContextManySpacesIdentity : IContextInitializeable
    {
        public ICollection<Space> SpacesFrom(ActivationContext context);
    }

    public abstract class ActivationContextManySpacesIdentityBase : ContextInitializeableBase, IActivationContextManySpacesIdentity
    {
        protected abstract ICollection<Space> AbstractSpacesFrom(ActivationContext context);

        public ICollection<Space> SpacesFrom(ActivationContext context)
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
            public IActivationContextSpaceIdentity firstSpace;
            public IActivationContextSpaceIdentity secondSpace;

            public IThreeSpaceRelationship thirdSpaceRelationship;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                firstSpace.Initialize(restrictionContext);
                secondSpace.Initialize(restrictionContext);
            }

            protected override ICollection<Space> AbstractSpacesFrom(ActivationContext context)
            {
                Space first = firstSpace.SpaceFrom(context);
                Space second = secondSpace.SpaceFrom(context);
                return Space.Spaces.Where(space => thirdSpaceRelationship.Evaluate(first, second, space)).ToArray();
            }
        }
    }
}