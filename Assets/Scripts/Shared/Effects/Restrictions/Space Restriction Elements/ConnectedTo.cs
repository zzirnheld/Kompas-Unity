using KompasCore.Effects.Identities;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Restrictions.SpaceRestrictionElements
{
    public class ConnectedTo : SpaceRestrictionElement
    {
        public IIdentity<IReadOnlyCollection<Space>> spaces;
        public IIdentity<IReadOnlyCollection<Space>> anyOfTheseSpaces;
        public SpaceRestriction byRestriction;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            if (spaces == null && anyOfTheseSpaces == null) throw new System.ArgumentNullException("spaces", "Failed to provide spaces for space restriction elements");

            spaces?.Initialize(initializationContext);
            anyOfTheseSpaces?.Initialize(initializationContext);
            byRestriction.Initialize(initializationContext);
        }

        protected override bool AbstractIsValidSpace(Space space, ActivationContext context)
        {
            return spaces.From(context, default)
                .All(s => InitializationContext.game.BoardController.AreConnectedBySpaces(s, space, byRestriction, context));
        }
    }
}