using KompasCore.Effects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace KompasClient.UI
{
    public class TriggerOrderUIController : MonoBehaviour
    {
        public GameObject triggerPrefab;

        public ClientUIController clientUICtrl;
        public Transform triggerObjectsParent;
        public Toggle autoOrderToggle;

        private readonly List<TriggerUIController> triggerUIs = new List<TriggerUIController>();

        public int CurrIndex { get; set; }

        public void OrderTriggers(IEnumerable<Trigger> triggers)
        {
            gameObject.SetActive(true);

            //eliminate any old triggers no longer needed. can't do .Except because types are wrong.
            var noLongerNeeded = triggerUIs.Where(t => !triggers.Contains(t.Trigger)).ToArray();
            foreach (var t in noLongerNeeded) Destroy(t);

            var toAdd = triggers.Except(triggerUIs.Select(t => t.Trigger));

            CurrIndex = 0;

            foreach (var t in toAdd)
            {
                var prefab = Instantiate(triggerPrefab, triggerObjectsParent);
                var ctrl = prefab.GetComponent<TriggerUIController>();
                ctrl.SetInfo(this, t.Source.SimpleSprite, t);
                triggerUIs.Add(ctrl);
            }

            if (autoOrderToggle.isOn) AutoOrder();
        }

        public void CancelOrder()
        {
            foreach (var t in triggerUIs) t.Index = -1;
        }

        public void AutoOrder()
        {
            foreach (var t in triggerUIs
                .Where(t => t.Index < 0)
                .OrderBy(t => t.Trigger.TriggerData.orderPriority)) //defaults to asending order
            {
                t.OnClick();
            }

            ConfirmOrder();
        }

        public void ConfirmOrder()
        {
            gameObject.SetActive(false);
            clientUICtrl.clientGame.clientNotifier.ChooseTriggerOrder(triggerUIs.Select(t => (t.Trigger, t.Index)));
            foreach (var ui in triggerUIs) Destroy(ui.gameObject);
            triggerUIs.Clear();
        }
    }
}