using KompasClient.UI;
using KompasCore.Cards;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasCore.UI
{
    public class ReminderTextParentUIController : MonoBehaviour
    {
        public const string RemindersJsonPath = "Reminder Text/Reminder Texts";

        public GameObject reminderPrefab;

        private readonly List<ReminderTextUIController> reminderCtrls
            = new List<ReminderTextUIController>();

        private ReminderTextsContainer reminders;

        public void Awake()
        {
            var jsonAsset = Resources.Load<TextAsset>(RemindersJsonPath);
            reminders = JsonConvert.DeserializeObject<ReminderTextsContainer>(jsonAsset.text);
            gameObject.SetActive(false);
        }

        public void ShowNothing() => gameObject.SetActive(false);

        public void ShowReminderText(GameCard card)
        {
            //clear existing reminders
            foreach (var reminderCtrl in reminderCtrls) Destroy(reminderCtrl.gameObject);
            reminderCtrls.Clear();
            //create new reminders
            foreach (var reminder in reminders.keywordReminderTexts)
            {
                if (card.EffText.Contains(reminder.keyword))
                {
                    var obj = Instantiate(reminderPrefab, gameObject.transform);
                    var ctrl = obj.GetComponent<ReminderTextUIController>();
                    ctrl.Initialize(reminder.keyword, reminder.reminder);
                    reminderCtrls.Add(ctrl);
                }
            }
            gameObject.SetActive(reminderCtrls.Any());
        }
    }
}