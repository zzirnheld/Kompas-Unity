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
        public bool Unique { get; private set; }

        public string Subtext { get; private set; }
        public string[] SpellSubtypes { get; private set; }
        public int Radius { get; private set; }
        public int Duration { get; private set; }
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
                if(CardType == 'A') return AugmentSubtypes == null ? "" : $"Augment: {string.Join(",", AugmentSubtypes)}";
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
                simpleSprite = Resources.Load<Sprite>("Simple Sprites/" + card.cardName);
            }
            //else Debug.Log("Names match. Set Info not updating pics.");

            Subtext = card.subtext;
            SpellSubtypes = card.spellTypes;
            Fast = card.fast;
            Unique = card.unique;
            Radius = card.radius;
            Duration = card.duration;
            CardType = card.cardType;
            CardName = card.cardName;
            EffText = card.effText;
            SubtypeText = card.subtypeText;
            AugmentSubtypes = card.augSubtypes;
        }

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
    }
}