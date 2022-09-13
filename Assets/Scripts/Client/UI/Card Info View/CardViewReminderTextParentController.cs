using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KompasCore.UI
{
    public class CardViewReminderTextParentController : MonoBehaviour
    {
        public GameObject reminderTextPrefab;

        private readonly List<CardViewReminderTextController> controllers = new List<CardViewReminderTextController>();

        public void Show(List<string> reminders)
        {
            foreach (var ctrl in controllers) Destroy(ctrl.gameObject);
            controllers.Clear();

            gameObject.SetActive(reminders.Count > 0);

            foreach(var reminder in reminders)
            {
                var go = Instantiate(reminderTextPrefab, transform);
                var ctrl = go.GetComponent<CardViewReminderTextController>();
                ctrl.reminderText.text = reminder;
                controllers.Add(ctrl);
            }
        }
    }
}