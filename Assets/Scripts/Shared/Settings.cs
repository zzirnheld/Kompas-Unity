
using Newtonsoft.Json;
using UnityEngine;

namespace KompasCore.GameCore
{
    public abstract class Settings
    {
        public static readonly Color32 DefaultFriendlyBlue = new Color32(74, 78, 156, 255);
        public static readonly Color32 DefaultEnemyRed = new Color32(255, 53, 49, 255);

        public static readonly Color32 FriendlyGold = new Color32(226, 166, 0, 255);
        public static readonly Color32 EnemySilver = new Color32(128, 128, 128, 255);
        public byte friendlyColorRed = DefaultFriendlyBlue.r;
        public byte friendlyColorGreen = DefaultFriendlyBlue.g;
        public byte friendlyColorBlue = DefaultFriendlyBlue.b;
        public byte enemyColorRed = DefaultEnemyRed.r;
        public byte enemyColorGreen = DefaultEnemyRed.g;
        public byte enemyColorBlue = DefaultEnemyRed.b;
        public int friendlyColorIndex = 0;
        public int enemyColorIndex = 0;

        [JsonIgnore]
        public Color32 FriendlyColor
        {
            set
            {
                friendlyColorRed = value.r;
                friendlyColorGreen = value.g;
                friendlyColorBlue = value.b;
                Debug.Log($"Setting friendly color to {value}. {friendlyColorRed}, {friendlyColorGreen}, {friendlyColorBlue}");
            }
            get
            {
                return new Color32(friendlyColorRed, friendlyColorGreen, friendlyColorBlue, 255);
            }
        }
        [JsonIgnore]
        public Color32 EnemyColor
        {
            set
            {
                enemyColorRed = value.r;
                enemyColorGreen = value.g;
                enemyColorBlue = value.b;
            }
            get
            {
                return new Color32(enemyColorRed, enemyColorGreen, enemyColorBlue, 255);
            }
        }
    }
}