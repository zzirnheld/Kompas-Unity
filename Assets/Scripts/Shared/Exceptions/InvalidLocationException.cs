using KompasCore.Cards;

namespace KompasCore.Exceptions
{
    public class InvalidLocationException : KompasException
    {
        public readonly CardLocation location;
        public readonly GameCard card;

        public InvalidLocationException(CardLocation location, GameCard card, string debugMessage = "", string message = "")
            : base(debugMessage, message)
        {
            this.location = location;
            this.card = card;
        }
    }
}