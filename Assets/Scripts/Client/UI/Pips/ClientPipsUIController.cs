using System.Collections.Generic;
using UnityEngine;

namespace KompasClient.UI
{
    public class ClientPipsUIController : MonoBehaviour
    {
        public int[] pipIntervals;
        public GameObject[] pipsPrefabs;
        public Vector3[] pipsPrefabsOffsets;

        private readonly List<GameObject> objsList = new List<GameObject>();

        private readonly List<PipRingsController> fivePipControllers = new List<PipRingsController>();

        private int numberOfOnePips;

        public int Pips
        {
            set => ShowPipsFor(value);
        }

        public void ShowPipsFor(int value)
        {
            foreach (var obj in objsList) Destroy(obj);
            objsList.Clear();

            fivePipControllers.Clear();
            numberOfOnePips = 0;

            Vector3 offset = Vector3.zero;
            for (int i = 0; i < pipIntervals.Length; i++)
            {
                int interval = pipIntervals[i];
                while (value >= interval)
                {
                    var obj = Instantiate(pipsPrefabs[i], parent: transform);
                    obj.transform.localPosition = offset;
                    obj.transform.rotation = Quaternion.identity;
                    offset += pipsPrefabsOffsets[i];
                    value -= interval;

                    if (interval == 5) fivePipControllers.Add(obj.GetComponent<PipRingsController>());
                    else if (interval == 1) numberOfOnePips++;

                    objsList.Add(obj);
                }
            }
        }

        public void HighlightPipsFor(int value)
        {
            Debug.Log($"Highlighitng {value} pips");
            if (value == 0 || (value > numberOfOnePips + (fivePipControllers.Count * 5)))
            {
                Debug.Log($"Bad value, showing no pips");
                foreach (var fpc in fivePipControllers)
                {
                    fpc.ShowRings(0);
                }
            }

            int toDisplay = value;

            //TODO later actually make the one pip rings spin.
            toDisplay -= numberOfOnePips;

            for (int i = 0; i < fivePipControllers.Count; i++)
            {
                if (toDisplay > 0)
                {
                    Debug.Log($"{toDisplay} rings left to show");
                    fivePipControllers[i].ShowRings(Mathf.Min(toDisplay, 5));
                    toDisplay -= 5;
                }
                else fivePipControllers[i].ShowRings(0);
            }
        }
    }
}