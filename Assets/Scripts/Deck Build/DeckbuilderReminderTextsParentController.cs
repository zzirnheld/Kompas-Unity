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
        public Transform remindersParent;
        public GameObject reminderPrefab;
        public ReminderTextsContainer Reminders => CardRepository.Reminders;

        public void Show(List<(string, string)> reminders)
        {
            //clear existing reminders
            foreach (var reminderCtrl in reminderCtrls) Destroy(reminderCtrl.gameObject);
            reminderCtrls.Clear();
            //create new reminders
            foreach (var (keyword, reminder) in reminders)
            {
                var obj = Instantiate(reminderPrefab, remindersParent);
                var ctrl = obj.GetComponent<ReminderTextUIController>();
                ctrl.Initialize(keyword, reminder);
                reminderCtrls.Add(ctrl);
            }
            remindersParent.gameObject.SetActive(reminderCtrls.Any());
        }
    }
}