using KompasCore.Cards;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasClient.UI
{
    public class ReminderTextParentClientUIController : MonoBehaviour
    {
        public const string RemindersJsonPath = "Reminder Text/Reminder Texts";

        public Transform remindersParent;
        public GameObject reminderPrefab;

        private readonly List<ReminderTextClientUIController> reminderCtrls
            = new List<ReminderTextClientUIController>();

        private ReminderTextsContainer reminders;

        public void Awake()
        {
            var jsonAsset = Resources.Load<TextAsset>(RemindersJsonPath);
            reminders = JsonConvert.DeserializeObject<ReminderTextsContainer>(jsonAsset.text);
            gameObject.SetActive(false);
        }

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
                    var obj = Instantiate(reminderPrefab, remindersParent);
                    var ctrl = obj.GetComponent<ReminderTextClientUIController>();
                    ctrl.Initialize(reminder.keyword, reminder.reminder);
                    reminderCtrls.Add(ctrl);
                }
            }
            remindersParent.gameObject.SetActive(reminderCtrls.Any());
        }
    }
}