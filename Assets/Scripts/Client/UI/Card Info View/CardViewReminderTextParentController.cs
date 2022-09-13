using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KompasCore.UI
{
    public interface IReminderTextParentController
    {
        public void Show(List<string> reminders);
    }

    public class CardViewReminderTextParentController : MonoBehaviour, IReminderTextParentController
    {
        public GameObject reminderTextPrefab;

        private readonly List<CardViewReminderTextController> controllers = new List<CardViewReminderTextController>();

        public void Show(List<string> reminders)
        {
            transform.position = Input.mousePosition;
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