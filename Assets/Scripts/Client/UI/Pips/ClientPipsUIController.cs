using KompasClient.GameCore;
using KompasCore.Cards;
using KompasCore.Helpers;
using System.Collections.Generic;
using System.Linq;
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
        public TMP_Text nextTurnPipsText;

        private readonly List<GameObject> objsList = new List<GameObject>();

        //private readonly Dictionary<int, List<PipRingsController>> pipControllers = new Dictionary<int, List<PipRingsController>>();
        public Dictionary<int, List<PipRingsController>> pipRingsControllers;

        private int pips;
        public int Pips
        {
            set
            {
                pips = value;
                ShowPipsFor(value);
            }
            get => pips;
        }

        private void Awake()
        {
            pipRingsControllers = new Dictionary<int, List<PipRingsController>>();

            foreach (var interval in pipIntervals)
            {
                Debug.Log($"Intervals added for {interval}");
                pipRingsControllers[interval] = new List<PipRingsController>();
            }

            Debug.Log($"Pip controllers are now {string.Join(", ", pipRingsControllers.Keys.Select(k => $"{k}"))} for  {gameObject.name} number {gameObject.GetHashCode()}");
        }

        /// <summary>
        /// Shows a number of pips currently owned by this player
        /// </summary>
        /// <param name="value"></param>
        private void ShowPipsFor(int value)
        {
            Debug.Log($"Updating player {player.index} pips to {value} while leyload is {player.clientGame.Leyload} and turn player is {player.clientGame.TurnPlayer.index}");
            pipsText.text = $"{value}";
            nextTurnPipsText.text = $"{player.clientGame.Leyload + (player.clientGame.TurnPlayer.index == player.index ? 2 : 1)}";

            foreach (var obj in objsList) Destroy(obj);
            objsList.Clear();

            foreach (var list in pipRingsControllers.Values) list.Clear();

            Vector3 offset = Vector3.zero;
            foreach(var (i, interval) in pipIntervals.Enumerate())
            {
                while (value >= interval)
                {
                    var obj = Instantiate(pipsPrefabs[i], parent: transform);
                    obj.transform.localPosition = offset;
                    obj.transform.rotation = Quaternion.identity;
                    offset += pipsPrefabsOffsets[i];
                    value -= interval;

                    pipRingsControllers[interval].Add(obj.GetComponent<PipRingsController>());

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
            if (value == 0 || (value > pips))
            {
                foreach(var list in pipRingsControllers.Values) foreach (var fpc in list)
                {
                    fpc.ShowRings(0);
                }
            }

            int toDisplay = value;

            foreach (int interval in pipIntervals)
            {
                var list = pipRingsControllers[interval];

                foreach (PipRingsController pipRings in list)
                {
                    if (toDisplay >= interval)
                    {
                        pipRings.ShowRings(interval);
                        toDisplay -= interval;
                    }
                    else
                    {
                        //Account for situations where you have more of the larger one and not enough of the smaller ones
                        //Don't pay with pennies if you have to break a nickel anyway
                        int remainingCapacity = pipRingsControllers
                            .Where(keyVal => keyVal.Key < interval)
                            .Select(keyVal => keyVal.Key * keyVal.Value.Count())
                            .Sum();

                        if (toDisplay > 0 && remainingCapacity < toDisplay)
                        {
                            pipRings.ShowRings(toDisplay);
                            toDisplay = 0;
                        }
                        else pipRings.ShowRings(0);

                    }
                }
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