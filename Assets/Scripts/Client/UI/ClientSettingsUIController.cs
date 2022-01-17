using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using TMPro;
using System;
using KompasClient.GameCore;

namespace KompasClient.UI
{
    public class ClientSettingsUIController : MonoBehaviour
    {
        public string ClientSettingsPath => Application.persistentDataPath + "/ClientUISettings.json";
        public ClientSettings ClientSettings { get; private set; }

        public ClientGame clientGame;
        public TMP_Dropdown statHighlightDropdown;
        public TMP_Dropdown confirmTargetsDropdown;
        public TMP_InputField zoomThresholdInput;

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

        public void Show()
        {
            statHighlightDropdown.ClearOptions();
            foreach(var o in Enum.GetValues(typeof(StatHighlight)))
            {
                statHighlightDropdown.options.Add(new TMP_Dropdown.OptionData() { text = o.ToString() });
            }
            confirmTargetsDropdown.ClearOptions();
            foreach (var o in Enum.GetValues(typeof(ConfirmTargets)))
            {
                confirmTargetsDropdown.options.Add(new TMP_Dropdown.OptionData() { text = o.ToString() });
            }

            statHighlightDropdown.value = (int) ClientSettings.statHighlight;
            confirmTargetsDropdown.value = (int) ClientSettings.confirmTargets;
            zoomThresholdInput.text = ClientSettings.zoomThreshold.ToString("n1");

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

        public void SetZoomThreshold(string thresholdString)
        {
            if (!float.TryParse(thresholdString, out ClientSettings.zoomThreshold)
                || ClientSettings.zoomThreshold < 5f)
                ClientSettings.zoomThreshold = ClientSettings.DefaultZoomThreshold;

            zoomThresholdInput.text = ClientSettings.zoomThreshold.ToString("n1");
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
        public string defaultIP;

        public static ClientSettings Default => new ClientSettings()
        {
            statHighlight = StatHighlight.NoHighlight,
            zoomThreshold = DefaultZoomThreshold,
            confirmTargets = ConfirmTargets.No,
            defaultIP = ""
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