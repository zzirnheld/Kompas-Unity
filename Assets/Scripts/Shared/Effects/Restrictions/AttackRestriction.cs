using KompasCore.Cards;

namespace KompasCore.Effects
{
    [System.Serializable]
    public class AttackRestriction
    {
        public const string ThisIsCharacter = "This is Character";
        public const string DefenderIsCharacter = "Defender is Character";
        public const string DefenderIsAdjacent = "Defender is Adjacent";
        public const string DefenderIsEnemy = "Defender is Enemy";
        public const string FriendlyTurn = "Friendly Turn";
        public const string MaxPerTurn = "Maximum Per Turn";
        public const string NothingResolving = "Nothing Resolving";

        public const string ThisIsActive = "This is Activated";

        public string[] attackRestrictions = new string[] { ThisIsCharacter, DefenderIsCharacter, DefenderIsAdjacent, DefenderIsEnemy, 
            FriendlyTurn, MaxPerTurn, NothingResolving };
        public int maxAttacks = 1;

        public GameCard Card { get; private set; }

        public void SetInfo(GameCard card)
        {
            Card = card;
        }

        public bool Evaluate(GameCard defender)
        {
            if (defender == null) return false;

            foreach (string r in attackRestrictions)
            {
                switch (r)
                {
                    case ThisIsCharacter:
                        if (Card.CardType != 'C') return false;
                        break;
                    case DefenderIsCharacter:
                        if (defender.CardType != 'C') return false;
                        break;
                    case DefenderIsAdjacent:
                        if (!Card.IsAdjacentTo(defender)) return false;
                        break;
                    case DefenderIsEnemy:
                        if (Card.Controller == defender.Controller) return false;
                        break;
                    case ThisIsActive:
                        if (!Card.Activated) return false;
                        break;
                    case FriendlyTurn:
                        if (Card.Controller != Card.Game.TurnPlayer) return false;
                        break;
                    case MaxPerTurn:
                        if (Card.AttacksThisTurn >= maxAttacks) return false;
                        break;
                    case NothingResolving:
                        if (Card.Game.CurrStackEntry != default) return false;
                        break;
                    default:
                        throw new System.ArgumentException($"Could not understand attack restriction {r}");
                }
            }

            return true;
        }
    }
}