using KompasCore.Cards;

namespace KompasCore.Effects
{
    public struct ActivationContext
    {
        public readonly GameCard Card;
        public readonly IStackable Stackable;
        public readonly Player Triggerer;
        public readonly int? X;
        public readonly (int, int)? Space;
        public readonly int StartIndex;

        public ActivationContext(GameCard card = null, IStackable stackable = null, Player triggerer = null,
            int? x = null, (int, int)? space = null, int startIndex = 0)
        {
            Card = card;
            Stackable = stackable;
            Triggerer = triggerer;
            X = x;
            Space = space;
            StartIndex = startIndex;
        }

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();

            sb.Append(Card == null ? "No triggering card, " : $"Card: {Card.CardName}");
            sb.Append(Stackable == null ? "No triggering stackable, " : $"Stackable from: {Stackable.Source.CardName}");
            sb.Append(Triggerer == null ? "No triggering player, " : $"Triggerer: {Triggerer.index}");
            sb.Append(X == null ? "No X, " : $"X: {X}");
            sb.Append(Space == null ? "No space, " : $"Space: {Space}");
            sb.Append($"Starting at {StartIndex}");

            return sb.ToString();
        }
    }
}