using KompasCore.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasDeckbuilder.UI
{
    public class DeckbuilderReminderTextsParentController : MonoBehaviour, IReminderTextParentController
    {
        private readonly List<ReminderTextUIController> reminderCtrls
            = new List<ReminderTextUIController>();
        public GameObject reminderPrefab;
        public ReminderTextsContainer Reminders => CardRepository.Reminders;

        public void Show(List<(string, string)> reminders) => Show(reminders, null);

        public void Show(List<(string, string)> reminders, string highlightedReminder)
        {
            //if (reminders.Count > 0) Debug.Log($"Showing reminders {reminders}");
            //clear existing reminders
            foreach (var reminderCtrl in reminderCtrls) Destroy(reminderCtrl.gameObject);
            reminderCtrls.Clear();
            //create new reminders
            foreach (var (keyword, reminder) in reminders)
            {
                var obj = Instantiate(reminderPrefab, transform);
                var ctrl = obj.GetComponent<ReminderTextUIController>();
                ctrl.Initialize(keyword, reminder, keyword == highlightedReminder);
                reminderCtrls.Add(ctrl);
            }
            gameObject.SetActive(reminderCtrls.Any());
        }
    }
}