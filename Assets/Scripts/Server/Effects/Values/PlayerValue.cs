using KompasCore.Exceptions;
using System;

namespace KompasCore.Effects
{
    public class PlayerValue
    {
        #region values
        public const string Pips = "Pips";
        public const string HandSize = "Hand Size";
        public const string MaxHandSize = "Max Hand Size";
        #endregion values

        public string value;
        public int multiplier = 1;
        public int divisor = 1;
        public int modifier = 0;

        public int GetValueOf(Player player)
        {
            if (player == null) throw new NullPlayerException("Cannot get value of null card");

            int intermediateValue = value switch
            {
                Pips => player.Pips,
                HandSize => player.handCtrl.HandSize,
                MaxHandSize => player.HandSizeLimit,

                _ => throw new ArgumentException($"Invalid value string {value}", "value"),
            };
            return intermediateValue * multiplier / divisor + modifier;
        }
    }
}