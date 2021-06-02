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

        public int Pips
        {
            set => ShowPipsFor(value);
        }

        public void ShowPipsFor(int value)
        {
            foreach(var obj in objsList) Destroy(obj);
            objsList.Clear();

            Vector3 offset = Vector3.zero;
            for (int i = 0; i < pipIntervals.Length; i++)
            {
                int interval = pipIntervals[i];
                while (value >= interval)
                {
                    var obj = Instantiate(pipsPrefabs[i], parent: transform);
                    obj.transform.localPosition = offset;
                    offset += pipsPrefabsOffsets[i];
                    value -= interval;
                    objsList.Add(obj);
                }
            }
        }
    }
}