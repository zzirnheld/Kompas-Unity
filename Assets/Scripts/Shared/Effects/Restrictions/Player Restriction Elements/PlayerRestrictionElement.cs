using KompasCore.Cards;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Restrictions
{
	public abstract class PlayerRestrictionElement : RestrictionBase<Player>, IRestriction<GameCardBase>
	{
		public bool IsValid(GameCardBase item, IResolutionContext context)
			=> IsValid(item.Controller, context);
	}

	namespace PlayerRestrictionElements
	{
		public class AllOf : AllOfBase<Player> { }

		public class Not : PlayerRestrictionElement
		{
			public IRestriction<Player> negated;

			public override void Initialize(EffectInitializationContext initializationContext)
			{
				base.Initialize(initializationContext);
				negated.Initialize(initializationContext);
			}

			protected override bool IsValidLogic(Player item, IResolutionContext context)
				=> !negated.IsValid(item, context);
		}

        public class DefaultActivationRestrictions : AllOf
        {
            public string[] locations = { CardLocation.Board.StringVersion() };

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                var defaultElements = new IRestriction<Player>[] {
                    new GamestateRestrictionElements.FriendlyTurn(),
                    new GamestateRestrictionElements.CardFitsRestriction() {
                        card = new Identities.Cards.ThisCardNow(),
                        cardRestriction = new CardRestrictionElements.AllOf() { elements = new IRestriction<GameCardBase>[] {
                            new CardRestrictionElements.Location() { locations = this.locations },
                            new CardRestrictionElements.Not() { negated = new CardRestrictionElements.Negated() }
                        } }
                    },
                    new GamestateRestrictionElements.NothingHappening(),
                	new GamestateRestrictionElements.Not() { negated = new GamestateRestrictionElements.EffectAlreadyTriggered() },
                    new PlayersMatch() { player = new Identities.Players.FriendlyPlayer() }
            	};
                base.Initialize(initializationContext);
            }
        }
    }
}