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
        public TMP_Dropdown confirmTargetsDropdown;
        public TMP_InputField zoomThresholdInput;

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
            }
            else
            {

                string settingsJson = File.ReadAllText(ClientUISettingsPath);
                try
                {
                    if (string.IsNullOrEmpty(settingsJson)) ClientUISettings = ClientUISettings.Default;
                    else ClientUISettings = JsonConvert.DeserializeObject<ClientUISettings>(settingsJson).Cleanup();
                }
                catch (ArgumentException a)
                {
                    Debug.LogError($"Failed to load settings.\n{a.Message}.\n{a.StackTrace}");
                    ClientUISettings = ClientUISettings.Default;
                }
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


            statHighlightDropdown.value = (int) ClientUISettings.statHighlight;
            confirmTargetsDropdown.value = (int) ClientUISettings.confirmTargets;
            zoomThresholdInput.text = ClientUISettings.zoomThreshold.ToString("n1");

            gameObject.SetActive(true);
        }

        public void SetStatHighlight(int index)
        {
            ClientUISettings.statHighlight = (StatHighlight)index;
            ApplySettings();
        }

        public void SetConfirmTargets(int index)
        {
            ClientUISettings.confirmTargets = (ConfirmTargets)index;
            ApplySettings();
        }

        public void SetZoomThreshold(string thresholdString)
        {
            if (!float.TryParse(thresholdString, out ClientUISettings.zoomThreshold)
                || ClientUISettings.zoomThreshold < 5f)
                ClientUISettings.zoomThreshold = ClientUISettings.DefaultZoomThreshold;

            zoomThresholdInput.text = ClientUISettings.zoomThreshold.ToString("n1");
            ApplySettings();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }

    public enum StatHighlight { NoHighlight, ColoredBack }
    public enum ConfirmTargets { No, Prompt }

    public class ClientUISettings
    {
        public const float DefaultZoomThreshold = 14f;

        public StatHighlight statHighlight;
        public float zoomThreshold;
        public ConfirmTargets confirmTargets;

        public static ClientUISettings Default => new ClientUISettings()
        {
            statHighlight = StatHighlight.NoHighlight,
            zoomThreshold = DefaultZoomThreshold,
            confirmTargets = ConfirmTargets.No
        };

        /// <summary>
        /// Updates any json-default values to their regular defaults
        /// </summary>
        /// <returns><see cref="this"/></returns>
        public ClientUISettings Cleanup()
        {
            if (zoomThreshold == default) zoomThreshold = DefaultZoomThreshold;

            return this;
        }
    }
}