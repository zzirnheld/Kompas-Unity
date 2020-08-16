using UnityEngine;

namespace KompasCore.Cards
{
    public abstract class CardBase : MonoBehaviour
    {
        public const string SimpleSubtype = "Simple";
        public const string DelayedSubtype = "Delayed";
        public const string RadialSubtype = "Radial";
        public const string VanishingSubtype = "Vanishing";

        public char CardType { get; private set; }
        public string CardName { get; private set; }
        public string EffText { get; private set; }
        public string SubtypeText { get; private set; }

        public Sprite detailedSprite;
        public Sprite simpleSprite;

        protected void SetInfo(SerializableCard card)
        {
            CardType = card.cardType;
            CardName = card.cardName;
            EffText = card.effText;
            SubtypeText = card.subtypeText;

            detailedSprite = Resources.Load<Sprite>("Detailed Sprites/" + CardName);
            simpleSprite = Resources.Load<Sprite>("Simple Sprites/" + CardName);
        }
    }
}