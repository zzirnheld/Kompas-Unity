using KompasCore.Effects;
using System;
using System.Linq;
using UnityEngine;

namespace KompasCore.Cards
{
    public abstract class CardBase : IComparable
    {
        public const string SimpleSubtype = "Simple";
        public const string EnchantSubtype = "Enchant";
        public const string DelayedSubtype = "Delayed";
        public const string RadialSubtype = "Radial";
        public const string VanishingSubtype = "Vanishing";
        public const string Nimbleness = "N";
        public const string Endurance = "E";
        public const string SummoningCost = "S";
        public const string Wounding = "W";
        public const string CastingCost = "C";
        public const string AugmentCost = "A";
        public const string CostStat = "Cost";

        #region stats
        private int n;
        private int e;
        private int s;
        private int w;
        private int c;
        private int a;
        /// <summary>
        /// Nimbleness - spaces moveable per turn
        /// </summary>
        public virtual int N
        {
            get => n < 0 ? 0 : n;
            private set => n = value;
        }
        /// <summary>
        /// Endurance - hit points
        /// </summary>
        public virtual int E
        {
            get => e < 0 ? 0 : e;
            private set => e = value;
        }
        /// <summary>
        /// Summoning cost - character's pip cost
        /// </summary>
        public virtual int S
        {
            get => s < 0 ? 0 : s;
            private set => s = value;
        }
        /// <summary>
        /// Wounding - damage
        /// </summary>
        public virtual int W
        {
            get => w < 0 ? 0 : w;
            private set => w = value;
        }
        /// <summary>
        /// Casting cost - spell's pip cost
        /// </summary>
        public virtual int C
        {
            get => c < 0 ? 0 : c;
            private set => c = value;
        }
        /// <summary>
        /// Augment cost - augment's pip cost
        /// </summary>
        public virtual int A
        {
            get => a < 0 ? 0 : a;
            private set => a = value;
        }

        public abstract int BaseN { get; }
        public abstract int BaseE { get; }
        public abstract int BaseS { get; }
        public abstract int BaseW { get; }
        public abstract int BaseC { get; }
        public abstract int BaseA { get; }

        public CardStats Stats => (N, E, S, W, C, A);

        public bool Unique { get; private set; }

        public string Subtext { get; private set; }
        public string[] SpellSubtypes { get; private set; }
        public int Radius { get; private set; }
        public int Duration { get; protected set; }
        public char CardType { get; private set; }
        public string CardName { get; private set; }
        public string EffText { get; private set; }
        public string SubtypeText { get; private set; }

        public string QualifiedSubtypeText => AttributesString + ArgsString + SubtypeText;

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
                if (CardType == 'S')
                {
                    return (SpellSubtypes.FirstOrDefault()) switch
                    {
                        RadialSubtype => $" Radius {Radius}",
                        DelayedSubtype => $" Delayed {Duration}",
                        VanishingSubtype => $" Vanishing {Duration}",
                        _ => "",
                    };
                }

                return "";
            }
        }
        public string AttributesString => $"{(Unique ? " Unique" : "")} ";
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

        public Sprite SimpleSprite { get; private set; }

        public virtual string FileName { get; set; }

        protected CardBase(CardStats stats,
                                       string subtext, string[] spellTypes,
                                       bool unique,
                                       int radius, int duration,
                                       char cardType, string cardName, string fileName,
                                       string effText,
                                       string subtypeText)
        {
            (n, e, s, w, c, a) = stats;

            FileName = fileName;
            SetInfo(null, subtext, spellTypes, unique, radius, duration, cardType, cardName, effText, subtypeText);
        }

        protected void SetInfo(CardStats? stats,
                                       string subtext, string[] spellTypes,
                                       bool unique,
                                       int radius, int duration,
                                       char cardType, string cardName,
                                       string effText,
                                       string subtypeText)
        {
            if (stats.HasValue) SetStats(stats.Value);

            //set sprites if they aren't already set correctly 
            //(check this by card name. cards should never have a pic that doesn't match their name)
            if (cardName != CardName)
            {
                //Debug.Log($"Names are different, changing card pics to match name {FileName}");
                SimpleSprite = Resources.Load<Sprite>($"Simple Sprites/{FileName}");
            }
            //else Debug.Log("Names match. Set Info not updating pics.");

            Subtext = subtext; //TODO un-deprecate and use as an override for constructed subtype text from the subtypes array
            SpellSubtypes = spellTypes;
            Unique = unique;
            Radius = radius;
            Duration = duration;
            CardType = cardType;
            CardName = cardName ?? throw new ArgumentNullException("cardName", $"A card is missing a name.");
            EffText = effText ?? throw new ArgumentNullException("effText", $"Card {CardName} is missing effect text");
            SubtypeText = subtypeText ?? string.Empty;
        }

        protected void SetInfo(SerializableCard serializableCard)
            => SetInfo((serializableCard.n, serializableCard.e, serializableCard.s, serializableCard.w, serializableCard.c, serializableCard.a),
                serializableCard.subtext, serializableCard.spellTypes,
                serializableCard.unique,
                serializableCard.radius, serializableCard.duration,
                serializableCard.cardType, serializableCard.cardName,
                serializableCard.effText, serializableCard.subtypeText);

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

        /// <summary>
        /// Shorthand for modifying a card's NESW all at once.
        /// On the server, this only notifies the clients of stat changes once.
        /// </summary>
        public virtual void SetCharStats(int n, int e, int s, int w, IStackable stackSrc = null)
        {
            SetN(n, stackSrc, onlyStatBeingSet: false);
            SetE(e, stackSrc, onlyStatBeingSet: false);
            SetS(s, stackSrc, onlyStatBeingSet: false);
            SetW(w, stackSrc, onlyStatBeingSet: false);
        }

        /// <summary>
        /// Shorthand for modifying a card's NESW all at once.
        /// On the server, this only notifies the clients of stat changes once.
        /// </summary>
        public void AddToCharStats(int n, int e, int s, int w, IStackable stackSrc = null)
            => SetCharStats(N + n, E + e, S + s, W + w, stackSrc: stackSrc);

        /// <summary>
        /// Shorthand for modifying a card's stats all at once.
        /// On the server, this only notifies the clients of stat changes once.
        /// </summary>
        public void AddToStats(CardStats buff, IStackable stackSrc = null)
            => SetStats(Stats + buff, stackSrc);

        public void SwapCharStats(GameCard other, bool swapN = true, bool swapE = true, bool swapS = true, bool swapW = true)
        {
            int[] aNewStats = new int[4];
            int[] bNewStats = new int[4];

            (aNewStats[0], bNewStats[0]) = swapN ? (other.N, N) : (N, other.N);
            (aNewStats[1], bNewStats[1]) = swapE ? (other.E, E) : (E, other.E);
            (aNewStats[2], bNewStats[2]) = swapS ? (other.S, S) : (S, other.S);
            (aNewStats[3], bNewStats[3]) = swapW ? (other.W, W) : (W, other.W);

            SetCharStats(aNewStats[0], aNewStats[1], aNewStats[2], aNewStats[3]);
            other.SetCharStats(bNewStats[0], bNewStats[1], bNewStats[2], bNewStats[3]);
        }
    }
}