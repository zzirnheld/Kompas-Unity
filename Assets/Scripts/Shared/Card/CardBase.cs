using KompasCore.Effects;
using System;
using System.Linq;
using UnityEngine;

namespace KompasCore.Cards
{
    public abstract class CardBase : MonoBehaviour, IComparable
    {
        public const string SimpleSubtype = "Simple";
        public const string EnchantSubtype = "Enchant";
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
            private set => n = value;
        }
        public virtual int E
        {
            get => e < 0 ? 0 : e;
            private set => e = value;
        }
        public virtual int S
        {
            get => s < 0 ? 0 : s;
            private set => s = value;
        }
        public virtual int W
        {
            get => w < 0 ? 0 : w;
            private set => w = value;
        }
        public virtual int C
        {
            get => c < 0 ? 0 : c;
            private set => c = value;
        }
        public virtual int A
        {
            get => a < 0 ? 0 : a;
            private set => a = value;
        }

        public CardStats Stats => (N, E, S, W, C, A);

        public bool Fast { get; private set; }
        public bool Unique { get; private set; }

        public string Subtext { get; private set; }
        public string[] SpellSubtypes { get; private set; }
        public int Radius { get; private set; }
        public int Duration { get; protected set; }
        public char CardType { get; private set; }
        public string CardName { get; private set; }
        public string EffText { get; private set; }
        public string SubtypeText { get; private set; }
        public string[] AugmentSubtypes { get; private set; }

        public string QualifiedSubtypeText => AttributesString + SubtypeText + ArgsString;

        public int Cost
        {
            get
            {
                return CardType switch
                {
                    'C' => S,
                    'S' => C,
                    'A' => A,
                    _ => throw new System.NotImplementedException($"Cost not implemented for card type {CardType}"),
                };
            }
        }
        private string ArgsString
        {
            get
            {
                if (CardType == 'A') return AugmentSubtypes == null ? "" : $"Augment: {string.Join(",", AugmentSubtypes)}";
                else if (CardType == 'S')
                {
                    return (SpellSubtypes.FirstOrDefault()) switch
                    {
                        RadialSubtype => $" {Radius} spaces",
                        DelayedSubtype => $" {Duration} turns",
                        VanishingSubtype => $" {Duration} turns",
                        _ => "",
                    };
                }

                return "";
            }
        }
        public string AttributesString => $"{(Fast ? " Fast" : "")}{(Unique ? " Unique" : "")} ";
        public string StatsString
        {
            get
            {
                return CardType switch
                {
                    'C' => $"N: {N} / E: {E} / S: {S} / W: {W}",
                    'S' => $"C {C}",
                    'A' => $"A {A}",
                    _ => throw new System.NotImplementedException($"Stats string not implemented for card type {CardType}"),
                };
            }
        }
        #endregion

        public Sprite simpleSprite;

        protected void SetInfo(CardStats stats,
                                       string subtext, string[] spellTypes,
                                       bool fast, bool unique,
                                       int radius, int duration,
                                       char cardType, string cardName,
                                       string effText,
                                       string subtypeText,
                                       string[] augSubtypes)
        {
            SetStats(stats);

            //set sprites if they aren't already set correctly 
            //(check this by card name. cards should never have a pic that doesn't match their name)
            if (cardName != CardName)
            {
                //Debug.Log($"Names are different, changing card pics to match name {card.cardName}");
                simpleSprite = Resources.Load<Sprite>($"Simple Sprites/{cardName}");
            }
            //else Debug.Log("Names match. Set Info not updating pics.");

            Subtext = subtext; //TODO un-deprecate and use as an override for constructed subtype text from the subtypes array
            SpellSubtypes = spellTypes;
            Fast = fast;
            Unique = unique;
            Radius = radius;
            Duration = duration;
            CardType = cardType;
            CardName = cardName ?? throw new ArgumentException($"A card is missing a name.");
            EffText = effText ?? throw new ArgumentException($"Card {CardName} is missing effect text");
            SubtypeText = subtypeText ?? string.Empty;
            AugmentSubtypes = augSubtypes; //Null indicates a lack of required augment subtypes
        }

        protected void SetCardInformation(SerializableCard card)
            => SetInfo((card.n, card.e, card.s, card.w, card.c, card.a),
                       card.subtext, card.spellTypes,
                       card.fast, card.unique,
                       card.radius, card.duration,
                       card.cardType, card.cardName,
                       card.effText,
                       card.subtypeText,
                       card.augSubtypes);

        public override string ToString()
        {
            if (CardName == null) return "Null Card";
            return $"{CardName}, {N}/{E}/{S}/{W}/{C}/{A}";
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            var other = obj as CardBase;
            if (other == null) throw new ArgumentException("Other object is not a CardBase!");

            int compare = CardName.CompareTo(other.CardName);
            if (compare != 0) return compare;

            compare = N.CompareTo(other.N);
            if (compare != 0) return compare;

            compare = E.CompareTo(other.E);
            if (compare != 0) return compare;

            compare = S.CompareTo(other.S);
            if (compare != 0) return compare;

            compare = W.CompareTo(other.W);
            if (compare != 0) return compare;

            compare = C.CompareTo(other.C);
            if (compare != 0) return compare;

            compare = A.CompareTo(other.A);
            if (compare != 0) return compare;

            return 0;
        }

        /* This must happen through setters, not properties, so that notifications and stack sending
         * can be managed as intended. */
        public virtual void SetN(int n, IStackable stackSrc, bool onlyStatBeingSet = true) => N = n;
        public virtual void SetE(int e, IStackable stackSrc, bool onlyStatBeingSet = true) => E = e;
        public virtual void SetS(int s, IStackable stackSrc, bool onlyStatBeingSet = true) => S = s;
        public virtual void SetW(int w, IStackable stackSrc, bool onlyStatBeingSet = true) => W = w;
        public virtual void SetC(int c, IStackable stackSrc, bool onlyStatBeingSet = true) => C = c;
        public virtual void SetA(int a, IStackable stackSrc, bool onlyStatBeingSet = true) => A = a;

        /// <summary>
        /// Shorthand for modifying a card's stats all at once.
        /// On the server, this only notifies the clients of stat changes once.
        /// </summary>
        public virtual void SetStats(CardStats stats, IStackable stackSrc = null)
        {
            SetN(stats.n, stackSrc, onlyStatBeingSet: false);
            SetE(stats.e, stackSrc, onlyStatBeingSet: false);
            SetS(stats.s, stackSrc, onlyStatBeingSet: false);
            SetW(stats.w, stackSrc, onlyStatBeingSet: false);
            SetC(stats.c, stackSrc, onlyStatBeingSet: false);
            SetA(stats.a, stackSrc, onlyStatBeingSet: false);
        }
    }
}