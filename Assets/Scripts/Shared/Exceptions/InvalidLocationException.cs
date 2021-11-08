using KompasCore.Cards;

namespace KompasCore.Exceptions
{
    public class InvalidLocationException : KompasException
    {
        public readonly CardLocation location;
        public readonly GameCard card;

        public InvalidLocationException(CardLocation location, GameCard card, string message = "")
            : base(message)
        {
            this.location = location;
            this.card = card;
        }
    }
}