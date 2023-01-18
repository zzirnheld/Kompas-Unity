using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using TMPro;
using System;
using KompasClient.GameCore;
using System.Linq;
using UnityEngine.UI;

namespace KompasClient.UI
{
    public class ClientSettingsUIController : MonoBehaviour
    {
        public static readonly Color32 DefaultFriendlyBlue = new Color32(74, 78, 156, 255);
        public static readonly Color32 DefaultEnemyRed = new Color32(255, 53, 49, 255);

        public static readonly Color32 FriendlyGold = new Color32(226, 166, 0, 255);
        public static readonly Color32 EnemySilver = new Color32(128, 128, 128, 255);

        public static readonly Color32[] DefaultFriendlyColorOptions = { DefaultFriendlyBlue, FriendlyGold };
        public static readonly Color32[] DefaultEnemyColorOptions = { DefaultEnemyRed, EnemySilver };
        public static readonly string[] DefaultFriendlyColorOptionNames = { "Blue", "Gold" }; //TODO make these swatches
        public static readonly string[] DefaultEnemyColorOptionNames = { "Red", "Silver" };

        public string ClientSettingsPath => Application.persistentDataPath + "/ClientUISettings.json";
        public ClientSettings ClientSettings { get; private set; }

        public ClientGame clientGame;
        public TMP_Dropdown statHighlightDropdown;
        public TMP_Dropdown friendlyColorOptionsDropdown;
        public TMP_Dropdown enemyColorOptionsDropdown;
        public TMP_InputField zoomThresholdInput;
        public Toggle showAdvancedEffectUIToggle;

        void Awake()
        {
            LoadSettings();
        }

        public void SaveAndClose()
        {
            ApplySettings();
            Hide();
        }

        public void ApplySettings()
        {
            clientGame.ApplySettings();
            SaveSettings();
        }

        public void LoadSettings()
        {
            if (!File.Exists(ClientSettingsPath))
            {
                ClientSettings = ClientSettings.Default;
                SaveSettings();
            }
            else
            {

                string settingsJson = File.ReadAllText(ClientSettingsPath);
                try
                {
                    if (string.IsNullOrEmpty(settingsJson)) ClientSettings = ClientSettings.Default;
                    else ClientSettings = JsonConvert.DeserializeObject<ClientSettings>(settingsJson).Cleanup();
                }
                catch (ArgumentException a)
                {
                    Debug.LogError($"Failed to load settings.\n{a.Message}.\n{a.StackTrace}");
                    ClientSettings = ClientSettings.Default;
                }
            }

            if (gameObject.activeSelf) Show();
        }

        public void SaveSettings()
        {
            try
            {
                string settingsJson = JsonConvert.SerializeObject(ClientSettings);
                File.WriteAllText(ClientSettingsPath, settingsJson);
            }
            catch (ArgumentException a)
            {
                Debug.LogError($"Failed to serialize settings.\n{a.Message}.\n{a.StackTrace}");
            }
            catch (IOException i)
            {
                Debug.LogError($"Failed to save settings.\n{i.Message}.\n{i.StackTrace}");
            }
        }

        private int GetFriendlyColorIndex(Color color)
            => DefaultFriendlyColorOptions.Contains(color) ? Array.IndexOf(DefaultFriendlyColorOptions, color) : 0;

        private int GetEnemyColorIndex(Color color)
            => DefaultEnemyColorOptions.Contains(color) ? Array.IndexOf(DefaultEnemyColorOptions, color) : 0;

        public void Show()
        {
            statHighlightDropdown.ClearOptions();
            foreach (var o in Enum.GetValues(typeof(StatHighlight)))
            {
                statHighlightDropdown.options.Add(new TMP_Dropdown.OptionData() { text = o.ToString() });
            }
            friendlyColorOptionsDropdown.ClearOptions();
            foreach (var o in DefaultFriendlyColorOptionNames)
            {
                friendlyColorOptionsDropdown.options.Add(new TMP_Dropdown.OptionData() { text = o });
            }
            enemyColorOptionsDropdown.ClearOptions();
            foreach (var o in DefaultEnemyColorOptionNames)
            {
                enemyColorOptionsDropdown.options.Add(new TMP_Dropdown.OptionData() { text = o });
            }

            statHighlightDropdown.value = (int)ClientSettings.statHighlight;
            friendlyColorOptionsDropdown.value = ClientSettings.friendlyColorIndex;
            enemyColorOptionsDropdown.value = ClientSettings.enemyColorIndex;
            zoomThresholdInput.text = ClientSettings.zoomThreshold.ToString("n1");
            showAdvancedEffectUIToggle.isOn = ClientSettings.showAdvancedEffectsSettings;

            gameObject.SetActive(true);
        }

        public void SetStatHighlight(int index)
        {
            ClientSettings.statHighlight = (StatHighlight)index;
            ApplySettings();
        }

        public void SetConfirmTargets(int index)
        {
            ClientSettings.confirmTargets = (ConfirmTargets)index;
            ApplySettings();
        }

        public void SetFriendlyColor(int index)
        {
            if (index < 0 || index >= DefaultFriendlyColorOptions.Length)
            {
                //TODO custom colors
                return;
            }
            ClientSettings.FriendlyColor = DefaultFriendlyColorOptions[index];
            ClientSettings.friendlyColorIndex = index;
            ApplySettings();
        }

        public void SetEnemyColor(int index)
        {
            if (index < 0 || index >= DefaultEnemyColorOptions.Length)
            {
                //TODO custom colors
                return;
            }
            ClientSettings.EnemyColor = DefaultEnemyColorOptions[index];
            ClientSettings.enemyColorIndex = index;
            ApplySettings();
        }

        public void SetZoomThreshold(string thresholdString)
        {
            if (!float.TryParse(thresholdString, out ClientSettings.zoomThreshold)
                || ClientSettings.zoomThreshold < 5f)
                ClientSettings.zoomThreshold = ClientSettings.DefaultZoomThreshold;

            zoomThresholdInput.text = ClientSettings.zoomThreshold.ToString("n1");
            ApplySettings();
        }

        public void SetShowAdvancedEffectResponseUI(bool show)
        {
            ClientSettings.showAdvancedEffectsSettings = show;
            ApplySettings();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }

    public enum StatHighlight { NoHighlight, ColoredBack }
    public enum ConfirmTargets { No, Prompt }

    public class ClientSettings
    {
        public const float DefaultZoomThreshold = 14f;

        public StatHighlight statHighlight;
        public float zoomThreshold;
        public ConfirmTargets confirmTargets;
        public bool showAdvancedEffectsSettings = false;
        public string defaultIP;
        public byte friendlyColorRed = ClientSettingsUIController.DefaultFriendlyBlue.r;
        public byte friendlyColorGreen = ClientSettingsUIController.DefaultFriendlyBlue.g;
        public byte friendlyColorBlue = ClientSettingsUIController.DefaultFriendlyBlue.b;
        public byte enemyColorRed = ClientSettingsUIController.DefaultEnemyRed.r;
        public byte enemyColorGreen = ClientSettingsUIController.DefaultEnemyRed.g;
        public byte enemyColorBlue = ClientSettingsUIController.DefaultEnemyRed.b;
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

        public static ClientSettings Default => new ClientSettings()
        {
            statHighlight = StatHighlight.NoHighlight,
            zoomThreshold = DefaultZoomThreshold,
            confirmTargets = ConfirmTargets.No,
            showAdvancedEffectsSettings = false,
            defaultIP = "",
            FriendlyColor = ClientSettingsUIController.DefaultFriendlyBlue,
            EnemyColor = ClientSettingsUIController.DefaultEnemyRed,
            friendlyColorIndex = 0,
            enemyColorIndex = 0
        };

        /// <summary>
        /// Updates any json-default values to their regular defaults
        /// </summary>
        /// <returns><see cref="this"/></returns>
        public ClientSettings Cleanup()
        {
            if (zoomThreshold == default) zoomThreshold = DefaultZoomThreshold;

            return this;
        }
    }
}