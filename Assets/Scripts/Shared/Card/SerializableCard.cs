using KompasCore.Effects;
using System;

namespace KompasCore.Cards
{
    [Serializable]
    public class SerializableCard
    {
        //card type
        public char cardType;

        //perma-values
        public string cardName;
        public string effText;
        public string subtypeText;

        public string[] keywords = new string[0];

        public MovementRestriction MovementRestriction = null;
        public AttackRestriction AttackRestriction = null;
        public PlayRestriction PlayRestriction = null;

        public int n;
        public int e;
        public int s;
        public int w;
        public int c;
        public string spellType;
        public int arg;
        public int a;
        public bool fast;
        public string subtext;
        public string[] augSubtypes;
    }
}