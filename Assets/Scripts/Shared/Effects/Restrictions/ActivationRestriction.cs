namespace KompasCore.Effects
{
	public class ActivationRestriction : Restrictions.PlayerRestrictionElements.AllOf
	{

		/* This exists to debug a card's activation restriction,
		 * but should not be usually used because it prints a ton whenever
		 * a card's effect buttons are considered, or when the game checks to see if a person has a response.
		public bool RestrictionValidWithDebug(string restriction, Player activator)
		{
			bool valid = RestrictionValid(restriction, activator);
			if (!valid) Debug.Log($"Card {Card.CardName} effect # {Effect.EffectIndex} activation restriction " +
				$"flouts restriction {restriction} for activator {activator.index}");
			return valid;
		} */

        public bool IsPotentiallyValidActivation(Player activator) => throw new System.NotImplementedException(); //TODO with strings to type names?
    }
}