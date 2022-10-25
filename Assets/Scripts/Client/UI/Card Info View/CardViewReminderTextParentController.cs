using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KompasCore.UI
{
    public interface IReminderTextParentController
    {
        public void Show(List<(string, string)> reminders);
    }

    public class CardViewReminderTextParentController : MonoBehaviour, IReminderTextParentController
    {
        public GameObject reminderTextPrefab;

        private readonly ISet<CardViewReminderTextController> controllers = new HashSet<CardViewReminderTextController>();

        private readonly ISet<(string, string)> reminderRequests = new HashSet<(string, string)>();
        private readonly ISet<(string, string)> reminders = new HashSet<(string, string)>();

        private void Update()
        {
            //Debug.Log($"{string.Join(", ", reminders)} ;;; {string.Join(", ", reminderRequests)}");
            if (reminders.Count == reminderRequests.Count && reminders.SetEquals(reminderRequests))
            {
                reminderRequests.Clear();
                return;
            }

            reminders.Clear();
            reminders.UnionWith(reminderRequests);

            reminderRequests.Clear();

            transform.position = Input.mousePosition;
            foreach (var ctrl in controllers) Destroy(ctrl.gameObject);
            controllers.Clear();

            //Debug.Log($"Trying to show {reminders.Count} reminders");
            //gameObject.SetActive(reminders.Count > 0);

            foreach (var (_, reminder) in reminders)
            {
                var go = Instantiate(reminderTextPrefab, transform);
                var ctrl = go.GetComponent<CardViewReminderTextController>();
                ctrl.reminderText.text = reminder;
                controllers.Add(ctrl);
            }
        }

        public void Show(List<(string, string)> reminders)
        {
            reminderRequests.UnionWith(reminders);
        }
    }
}