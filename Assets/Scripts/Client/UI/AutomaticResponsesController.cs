using UnityEngine;

namespace KompasClient.UI
{
    public class AutomaticResponsesController : MonoBehaviour
    {
        public GameObject[] autoResponses;

        public void Show()
        {
            foreach(var o in autoResponses) o.SetActive(true);
        }

        public void Hide()
        {
            foreach (var o in autoResponses) o.SetActive(false);
        }

        public void Show(bool show) { if (show) Show(); else Hide(); }
    }
}