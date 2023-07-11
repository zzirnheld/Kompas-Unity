using System.Collections.Generic;
using KompasCore.Cards;

namespace KompasCore.Effects.Restrictions
{
	public interface IActivationRestriction : IRestriction<Player>, IAllOf<Player>
	{
	}

	public static class IActivationRestrictionExtensions
	{
		public static bool IsPotentiallyValidActivation(this IActivationRestriction restriction, Player activator)
			=> restriction.IsValidIgnoring(activator, default,
				restriction => restriction is not GamestateRestrictionElements.NothingHappening);
	}

	namespace PlayerRestrictionElements
	{
		public class ActivationRestriction : AllOf, IActivationRestriction
		{
			public string[] locations = { CardLocation.Board.StringVersion() };

			protected override IEnumerable<IRestriction<Player>> DefaultElements
			{
				get
				{
					yield return new GamestateRestrictionElements.FriendlyTurn();
					yield return new TriggerRestrictionElements.CardFitsRestriction()
					{
						card = new Identities.Cards.ThisCardNow(),
						cardRestriction = new CardRestrictionElements.AllOf()
						{
							elements = new IRestriction<GameCardBase>[] {
								new CardRestrictionElements.Location() { locations = this.locations },
								new CardRestrictionElements.Not() { negated = new CardRestrictionElements.Negated() }
							}
						}
					};
					yield return new GamestateRestrictionElements.NothingHappening();
					yield return new GamestateRestrictionElements.Not() { negated = new GamestateRestrictionElements.EffectAlreadyTriggered() };
					yield return new PlayersMatch() { player = new Identities.Players.FriendlyPlayer() };
				}
			}
		}
	}
}