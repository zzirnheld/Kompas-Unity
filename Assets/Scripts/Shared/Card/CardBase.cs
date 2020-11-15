using UnityEngine;

namespace KompasCore.Cards
{
    public abstract class CardBase : MonoBehaviour
    {
        public const string SimpleSubtype = "Simple";
        public const string DelayedSubtype = "Delayed";
        public const string RadialSubtype = "Radial";
        public const string VanishingSubtype = "Vanishing";

        #region stats
        private int n;
        private int e;
        private int s;
        private int w;
        private int c;
        private int a;
        public virtual int N
        {
            get => n < 0 ? 0 : n;
            protected set => n = value;
        }
        public virtual int E
        {
            get => e < 0 ? 0 : e;
            protected set => e = value;
        }
        public virtual int S
        {
            get => s < 0 ? 0 : s;
            protected set => s = value;
        }
        public virtual int W
        {
            get => w < 0 ? 0 : w;
            protected set => w = value;
        }
        public virtual int C
        {
            get => c < 0 ? 0 : c;
            protected set => c = value;
        }
        public virtual int A
        {
            get => a < 0 ? 0 : a;
            protected set => a = value;
        }

        public bool Fast { get; private set; }

        public string Subtext { get; private set; }
        public string SpellSubtype { get; private set; }
        public int Arg { get; private set; }
        public char CardType { get; private set; }
        public string CardName { get; private set; }
        public string EffText { get; private set; }
        public string SubtypeText { get; private set; }
        public string[] AugmentSubtypes { get; private set; }

        public int Cost
        {
            get
            {
                switch (CardType)
                {
                    case 'C': return S;
                    case 'S': return C;
                    case 'A': return A;
                    default: throw new System.NotImplementedException($"Cost not implemented for card type {CardType}");
                }
            }
        }
        private string SpellSubtypeString
        {
            get
            {
                if (CardType != 'S') return "";
                switch (SpellSubtype)
                {
                    case RadialSubtype: return $" {Arg}";
                    case DelayedSubtype: return $" {Arg} turns";
                    default: return "";
                }
            }
        }
        public string StatsString
        {
            get
            {
                switch (CardType)
                {
                    case 'C': return $"N: {N} / E: {E} / S: {S} / W: {W}";
                    case 'S': return $"C {C}{(Fast ? " Fast " : " ")}{SpellSubtype}{SpellSubtypeString}";
                    case 'A': return $"A {A}{(Fast ? " Fast " : " ")}{(AugmentSubtypes != null ? $"Augment: {string.Join(",", AugmentSubtypes)}" : "")}";
                    default: throw new System.NotImplementedException($"Stats string not implemented for card type {CardType}");
                }
            }
        }

        #endregion

        public Sprite detailedSprite;
        public Sprite simpleSprite;

        protected void SetInfo(SerializableCard card)
        {
            N = card.n;
            E = card.e;
            S = card.s;
            W = card.w;
            C = card.c;
            A = card.a;

            //set sprites if they aren't already set correctly 
            //(check this by card name. cards should never have a pic that doesn't match their name)
            if (card.cardName != CardName)
            {
                //Debug.Log($"Names are different, changing card pics to match name {card.cardName}");
                detailedSprite = Resources.Load<Sprite>("Detailed Sprites/" + card.cardName);
                simpleSprite = Resources.Load<Sprite>("Simple Sprites/" + card.cardName);
                if (detailedSprite == null) detailedSprite = simpleSprite;
                if (simpleSprite == null) simpleSprite = detailedSprite;
            }
            //else Debug.Log("Names match. Set Info not updating pics.");

            Subtext = card.subtext;
            SpellSubtype = card.spellType;
            Fast = card.fast;
            Arg = card.arg;
            CardType = card.cardType;
            CardName = card.cardName;
            EffText = card.effText;
            SubtypeText = card.subtypeText;
            AugmentSubtypes = card.augSubtypes;
        }

        public override string ToString()
        {
            return $"Card named \"{CardName}\"";
        }
    }
}