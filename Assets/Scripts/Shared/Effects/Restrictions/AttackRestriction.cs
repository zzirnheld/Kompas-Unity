using KompasCore.Cards;
using System.Linq;

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
        public const string StackEmpty = "Stack Empty";

        public const string ThisIsActive = "This is Activated";

        public string[] attackRestrictions = new string[] { ThisIsCharacter, DefenderIsCharacter, DefenderIsAdjacent, DefenderIsEnemy, 
            FriendlyTurn, MaxPerTurn, StackEmpty };
        public int maxAttacks = 1;

        public GameCard Card { get; private set; }

        public void SetInfo(GameCard card)
        {
            Card = card;
        }

        private bool RestrictionValid(string restriction, GameCard defender)
        {
            switch (restriction)
            {
                case ThisIsCharacter: return Card.CardType == 'C';
                case DefenderIsCharacter: return defender.CardType == 'C';
                case DefenderIsAdjacent: return Card.IsAdjacentTo(defender);
                case DefenderIsEnemy: return Card.Controller != defender.Controller;
                case FriendlyTurn: return Card.Controller == Card.Game.TurnPlayer;
                case MaxPerTurn: return Card.AttacksThisTurn < maxAttacks;
                case StackEmpty: return Card.Game.StackEmpty;
                case ThisIsActive: return Card.Activated;
                default: throw new System.ArgumentException($"Could not understand attack restriction {restriction}");
            }
        }

        public bool Evaluate(GameCard defender)
        {
            if (defender == null) return false;
            return attackRestrictions.All(r => RestrictionValid(r, defender));
        }
    }
}