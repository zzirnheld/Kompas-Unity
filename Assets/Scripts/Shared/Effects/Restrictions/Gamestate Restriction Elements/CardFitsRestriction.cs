using KompasCore.Effects.Identities;

namespace KompasCore.Effects.Restrictions.GamestateRestrictionElements
{
    public class CardFitsRestriction : Restrictions.TriggerRestrictionElements.CardFitsRestriction,
        IGamestateRestriction, IRestriction<Player>
    {
        public bool IsValid(IResolutionContext context) => IsValid(default, context);
        public bool IsValid(Player item, IResolutionContext context) => IsValid(context);


        //To get behavior consistent with gamestate "card fits restriction" (in which case there's no triggering context),
        //have to override ContextToConsider and FromIdentity
        protected override IResolutionContext ContextToConsider(TriggeringEventContext triggeringContext, IResolutionContext resolutionContext)
            => resolutionContext;

        protected override IdentityType FromIdentity<IdentityType>
            (IIdentity<IdentityType> identity, TriggeringEventContext triggeringEventContext, IResolutionContext resolutionContext)
            => identity.From(resolutionContext);
    }
}