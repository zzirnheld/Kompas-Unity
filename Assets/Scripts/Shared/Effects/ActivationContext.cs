using KompasCore.Cards;
using System.Collections.Generic;

namespace KompasCore.Effects
{
    public class ActivationContext
    {
        public readonly IGameCardInfo CardInfo;
        public readonly IStackable Stackable;
        public readonly Player Triggerer;
        public readonly int? X;
        public readonly Space? Space;
        // These two are for delayed
        public readonly int StartIndex;
        public readonly List<GameCard> Targets;

        public ActivationContext(GameCard card, IStackable stackable = null, Player triggerer = null,
            int? x = null, Space? space = null, int startIndex = 0, List<GameCard> targets = null)
            : this(card == null ? null : new GameCardInfo(card), stackable, triggerer, x, space, startIndex, targets ?? new List<GameCard>())
        { }

        public ActivationContext(IGameCardInfo card = null, IStackable stackable = null, Player triggerer = null,
            int? x = null, Space? space = null, int startIndex = 0, List<GameCard> targets = null)
        {
            CardInfo = card;
            Stackable = stackable;
            Triggerer = triggerer;
            X = x;
            Space = space;
            StartIndex = startIndex;
            Targets = targets;
        }

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();

            sb.Append(CardInfo == null ? "No triggering card, " : $"Card: {CardInfo.CardName}, ");
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