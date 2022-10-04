using KompasClient.GameCore;
using KompasCore.Cards;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KompasClient.UI
{
    public class ClientPipsUIController : MonoBehaviour
    {
        public ClientPlayer player;

        [Header("Pip info")]
        [Tooltip("The number of pips each of the prefabs represents")]
        public int[] pipIntervals;
        [Tooltip("The prefab for each of those numbers of pips")]
        public GameObject[] pipsPrefabs;
        [Tooltip("The vector by which each pip prefab should be offset from the last")]
        public Vector3[] pipsPrefabsOffsets;

        public TMP_Text pipsText;
        public string pipsTextPrefix;
        public TMP_Text nextTurnPipsText;

        private readonly List<GameObject> objsList = new List<GameObject>();
        private readonly List<PipRingsController> fivePipControllers = new List<PipRingsController>();
        private int numberOfOnePips;

        public int Pips
        {
            set => ShowPipsFor(value);
        }

        /// <summary>
        /// Shows a number of pips currently owned by this player
        /// </summary>
        /// <param name="value"></param>
        public void ShowPipsFor(int value)
        {
            pipsText.text = $"{pipsTextPrefix}{value}";
            nextTurnPipsText.text = $"(+{player.clientGame.Leyload + (player.clientGame.FriendlyTurn ? 2 : 1)} next turn)";

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

        /// <summary>
        /// Highlights a given number of pips,
        /// like showning how many pips something would cost.
        /// </summary>
        /// <param name="value"></param>
        public void HighlightPipsFor(int value)
        {
            //Debug.Log($"Highlighitng {value} pips");
            if (value == 0 || (value > numberOfOnePips + (fivePipControllers.Count * 5)))
            {
                //Debug.Log($"Bad value {value}, showing no pips");
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
                    //Debug.Log($"{toDisplay} rings left to show");
                    fivePipControllers[i].ShowRings(Mathf.Min(toDisplay, 5));
                    toDisplay -= 5;
                }
                else fivePipControllers[i].ShowRings(0);
            }
        }

        /// <summary>
        /// Shows how many pips that card would cost to play,
        /// if that card is in the right place
        /// </summary>
        /// <param name="card"></param>
        public void HighlightPipsFor(GameCard card)
        {
            bool showCost = card != null
                && card.Location == CardLocation.Hand
                && card.Cost <= card.Controller.Pips;
            int costToHighlight = showCost ? card.Cost : 0;
            HighlightPipsFor(costToHighlight);
        }
    }
}