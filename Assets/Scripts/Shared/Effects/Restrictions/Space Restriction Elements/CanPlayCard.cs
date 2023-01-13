using KompasCore.Cards;
using KompasCore.Effects.Identities;

namespace KompasCore.Effects.Restrictions.SpaceRestrictionElements
{
    /// <summary>
    /// Whether a card can be moved to that space. Presumes from effect
    /// </summary>
    public class CanPlayCard : SpaceRestrictionElement
    {
        public INoActivationContextIdentity<GameCardBase> toPlay;

        public bool normalPlay = false;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            toPlay.Initialize(initializationContext);
        }

        protected override bool AbstractIsValidSpace(Space space, ActivationContext context)
            => normalPlay
            ? toPlay.Item.Card.PlayRestriction.IsValidNormalPlay(space, InitializationContext.Controller)
            : toPlay.Item.Card.PlayRestriction.IsValidEffectPlay(space, InitializationContext.effect, InitializationContext.Controller, context);
    }
}