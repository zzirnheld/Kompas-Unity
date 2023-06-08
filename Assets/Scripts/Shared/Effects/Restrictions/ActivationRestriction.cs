﻿namespace KompasCore.Effects
{
	public interface IActivationRestriction : IRestriction<Player>, IAllOf<Player>
	{
	}

	public static class IActivationRestrictionExtensions
	{
		public static bool IsPotentiallyValidActivation(this IActivationRestriction restriction, Player activator)
			=> restriction.IsValidIgnoring(activator, default,
				restriction => restriction is not Restrictions.GamestateRestrictionElements.NothingHappening);
	}
}