using KompasCore.Cards;
using System.Collections.Generic;

namespace KompasCore.Effects
{
    public class ActivationContext
    {
        public readonly IGameCardInfo BeforeCardInfo;
        public readonly IStackable Stackable;
        public readonly Player Triggerer;
        public readonly int? X;
        public readonly Space Space;
        // These two are for delayed
        public readonly int StartIndex;
        public readonly List<GameCard> Targets;
        public readonly List<Space> Spaces;

        public IGameCardInfo AfterCardInfo { get; private set; }

        public ActivationContext(GameCard beforeCard, IStackable stackable = null, Player triggerer = null,
            int? x = null, Space space = null, int startIndex = 0, List<GameCard> targets = null, List<Space> spaces = null)
            : this(beforeCard == null ? null : new GameCardInfo(beforeCard), 
                  stackable, triggerer, x, space, startIndex, targets ?? new List<GameCard>(), spaces ?? new List<Space>())
        { }

        public ActivationContext(IGameCardInfo beforeCard = null, IStackable stackable = null, Player triggerer = null,
            int? x = null, Space? space = null, int startIndex = 0, List<GameCard> targets = null, List<Space> spaces = null)
        {
            BeforeCardInfo = beforeCard;
            Stackable = stackable;
            Triggerer = triggerer;
            X = x;
            Space = space?.Copy;
            StartIndex = startIndex;
            Targets = targets;
            this.Spaces = spaces;
        }

        public void SetAfterCardInfo(GameCard afterCard)
        {
            if (AfterCardInfo != null) throw new System.ArgumentException("Already initialized AfterCardInfo on this context");
            AfterCardInfo = new GameCardInfo(afterCard);
        }

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();

            sb.Append(BeforeCardInfo == null ? "No triggering card, " : $"Card: {BeforeCardInfo.CardName}, ");
            sb.Append(Stackable == null ? "No triggering stackable, " : $"Stackable from: {Stackable.Source?.CardName}, ");
            sb.Append(Triggerer == null ? "No triggering player, " : $"Triggerer: {Triggerer.index}, ");
            sb.Append(X == null ? "No X, " : $"X: {X}, ");
            sb.Append(Space == null ? "No space, " : $"Space: {Space}, ");
            sb.Append(Targets == null ? "No targets, " : $"Targets: {string.Join(", ", Targets)}, ");
            sb.Append($"Starting at {StartIndex}");

            return sb.ToString();
        }
    }
}