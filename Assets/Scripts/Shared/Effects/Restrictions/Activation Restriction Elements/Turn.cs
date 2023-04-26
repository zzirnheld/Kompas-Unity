namespace KompasCore.Effects.Restrictions.GamestateRestrictionElements
{
	public abstract class Turn : GamestateRestrictionBase
	{
		//If end up needing a version that can leverage trigger restriction elements, will need to split this back out to trigger/gamestate versions
		protected abstract Player TurnPlayer { get; }

		protected override bool IsValidLogic()
			=> InitializationContext.game.TurnPlayer == TurnPlayer;
	}

	public class FriendlyTurn : Turn
	{
		protected override Player TurnPlayer => new Identities.Players.FriendlyPlayer().Item;
	}

	public class EnemyTurn : Turn
	{
		protected override Player TurnPlayer => new Identities.Players.EnemyPlayer().Item;
	}
}