using KompasCore.Cards;
using System.Collections.Generic;

namespace KompasCore.Effects
{
    public struct ActivationContext
    {
        public readonly GameCard Card;
        public readonly IStackable Stackable;
        public readonly Player Triggerer;
        public readonly int? X;
        public readonly (int, int)? Space;
        // These two are for delayed
        public readonly int StartIndex;
        public readonly List<GameCard> Targets;

        public ActivationContext(GameCard card = null, IStackable stackable = null, Player triggerer = null,
            int? x = null, (int, int)? space = null, int startIndex = 0, List<GameCard> targets = null)
        {
            Card = card;
            Stackable = stackable;
            Triggerer = triggerer;
            X = x;
            Space = space;
            StartIndex = startIndex;
            Targets = targets; //no use ??ing to a new list because there's the default parameterless constructor
        }

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();

            sb.Append(Card == null ? "No triggering card, " : $"Card: {Card.CardName}, ");
            sb.Append(Stackable == null ? "No triggering stackable, " : $"Stackable from: {Stackable.Source.CardName}, ");
            sb.Append(Triggerer == null ? "No triggering player, " : $"Triggerer: {Triggerer.index}, ");
            sb.Append(X == null ? "No X, " : $"X: {X}, ");
            sb.Append(Space == null ? "No space, " : $"Space: {Space}, ");
            sb.Append(Targets == null ? "No targets, " : $"Targets: {string.Join(", ", Targets)}, ");
            sb.Append($"Starting at {StartIndex}");

            return sb.ToString();
        }
    }
}