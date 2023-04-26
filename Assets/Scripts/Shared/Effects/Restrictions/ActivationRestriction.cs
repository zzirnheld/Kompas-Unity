namespace KompasCore.Effects
{
	public class ActivationRestriction : Restrictions.PlayerRestrictionElements.AllOf
	{
        public bool IsPotentiallyValidActivation(Player activator)
            => IsValidIgnoring(activator, default,
                restriction => restriction is not Restrictions.GamestateRestrictionElements.NothingHappening);
    }
}