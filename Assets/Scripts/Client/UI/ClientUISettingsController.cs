using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using TMPro;
using System;
using KompasClient.GameCore;

namespace KompasClient.UI
{
    public class ClientUISettingsController : MonoBehaviour
    {
        public string ClientUISettingsPath => Application.persistentDataPath + "/ClientUISettings.json";

        public ClientUISettings ClientUISettings { get; private set; }

        public ClientGame clientGame;
        public TMP_Dropdown statHighlightDropdown;

        public void ApplySettings()
        {
            clientGame.ApplySettings();
            SaveSettings();
        }

        public void LoadSettings()
        {
            if (!File.Exists(ClientUISettingsPath))
            {
                ClientUISettings = ClientUISettings.Default;
                SaveSettings();
                return;
            }

            string settingsJson = File.ReadAllText(ClientUISettingsPath);
            try
            {
                if (string.IsNullOrEmpty(settingsJson)) ClientUISettings = ClientUISettings.Default;
                else ClientUISettings = JsonConvert.DeserializeObject<ClientUISettings>(settingsJson);
            }
            catch (ArgumentException a)
            {
                Debug.LogError($"Failed to load settings.\n{a.Message}.\n{a.StackTrace}");
                ClientUISettings = ClientUISettings.Default;
            }

            if (gameObject.activeSelf) Show();
        }

        public void SaveSettings()
        {
            try
            {
                string settingsJson = JsonConvert.SerializeObject(ClientUISettings);
                File.WriteAllText(ClientUISettingsPath, settingsJson);
            }
            catch (System.ArgumentException a)
            {
                Debug.LogError($"Failed to serialize settings.\n{a.Message}.\n{a.StackTrace}");
            }
            catch (IOException i)
            {
                Debug.LogError($"Failed to save settings.\n{i.Message}.\n{i.StackTrace}");
            }
        }

        public void Show()
        {
            statHighlightDropdown.ClearOptions();
            foreach(var o in Enum.GetValues(typeof(StatHighlight)))
            {
                statHighlightDropdown.options.Add(new TMP_Dropdown.OptionData() { text = o.ToString() });
            }
            statHighlightDropdown.value = (int) ClientUISettings.statHighlight;

            gameObject.SetActive(true);
        }

        public void SetStatHighlight(int index)
        {
            ClientUISettings.statHighlight = (StatHighlight)index;
            ApplySettings();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }

    public enum StatHighlight { NoHighlight, ColoredBack }

    [System.Serializable]
    public class ClientUISettings
    {
        public StatHighlight statHighlight;

        public static ClientUISettings Default => new ClientUISettings()
        {
            statHighlight = StatHighlight.NoHighlight
        };


    }
}