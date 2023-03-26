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

        public static readonly Color32[] DefaultFriendlyColorOptions = { ClientSettings.DefaultFriendlyBlue, ClientSettings.FriendlyGold };
        public static readonly Color32[] DefaultEnemyColorOptions = { ClientSettings.DefaultEnemyRed, ClientSettings.EnemySilver };
        public static readonly string[] DefaultFriendlyColorOptionNames = { "Blue", "Gold" }; //TODO make these swatches
        public static readonly string[] DefaultEnemyColorOptionNames = { "Red", "Silver" };

        public string ClientSettingsPath => Application.persistentDataPath + "/ClientUISettings.json";
        public ClientSettings clientSettings;
        public ClientSettings ClientSettings 
        {
            get {
                if (clientSettings == default) clientSettings = LoadSettings();
                return clientSettings;
            }
        }

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

        public ClientSettings LoadSettings()
        {
            ClientSettings ret;
            if (!File.Exists(ClientSettingsPath))
            {
                ret = ClientSettings.Default;
                SaveSettings();
            }
            else
            {

                string settingsJson = File.ReadAllText(ClientSettingsPath);
                try
                {
                    if (string.IsNullOrEmpty(settingsJson)) ret = ClientSettings.Default;
                    else ret = JsonConvert.DeserializeObject<ClientSettings>(settingsJson).Cleanup();
                }
                catch (ArgumentException a)
                {
                    Debug.LogError($"Failed to load settings.\n{a.Message}.\n{a.StackTrace}");
                    ret = ClientSettings.Default;
                }
            }

            if (gameObject.activeSelf) Show();
            return ret;
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
}