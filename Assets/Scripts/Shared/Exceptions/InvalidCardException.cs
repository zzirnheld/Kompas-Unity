using KompasCore.Cards;

namespace KompasCore.Exceptions
{
	public class InvalidCardException : KompasException
	{
		public readonly GameCard card;

		public InvalidCardException(GameCard card, string debugMessage = "", string message = "")
			: base(debugMessage, message)
		{
			this.card = card;
		}
	}
}