using KompasCore.Cards;
using KompasCore.GameCore;
using System;
using UnityEngine;

namespace KompasCore.UI
{
    public class BoardUIController : MonoBehaviour
    {
        public GameObject spaceCueControllerPrefab;
        public BoardController boardCtrl;

        private readonly SpaceCueController[,] spaceCueControllers = new SpaceCueController[7, 7];
        private GameCard currShowingFor;

        private static Vector3 GridIndicesToCuePos(int x, int y)
        {
            return new Vector3(BoardController.GridIndexToPos(x), 0.01f, BoardController.GridIndexToPos(y));
        }

        private void Awake()
        {
            for(int i = 0; i < 7; i++)
            {
                for(int j = 0; j < 7; j++)
                {
                    GameObject cue = Instantiate(spaceCueControllerPrefab, transform);
                    cue.transform.localPosition = GridIndicesToCuePos(i, j);
                    spaceCueControllers[i, j] = cue.GetComponent<SpaceCueController>();
                }
            }
        }

        public void ShowForCard(GameCard card, bool forceRefresh = false)
        {
            if (currShowingFor == card && !forceRefresh) return;

            currShowingFor = card;

            for(int i = 0; i < 7; i++)
            {
                for(int j = 0; j < 7; j++)
                {
                    var cue = spaceCueControllers[i, j];

                    if (card.MovementRestriction.EvaluateNormalMove(i, j))
                        cue.ShowCanMove();
                    else if (card.AttackRestriction.Evaluate(boardCtrl.GetCardAt(i, j)))
                        cue.ShowCanAttack();
                    else if (card.PlayRestriction.EvaluateNormalPlay(i, j, card.Controller, checkCanAffordCost: true))
                        cue.ShowCanPlay();
                    else
                        cue.ShowCanNone();
                }
            }
        }

        public void RefreshShownCard() => ShowForCard(currShowingFor, forceRefresh: true);

        public void ShowNothing()
        {
            foreach (var cue in spaceCueControllers) cue.ShowCanNone();
        }

        public void ShowSpaceTargets(Func<(int, int), bool> predicate)
        {
            for(int x = 0; x < 7; x++)
            {
                for(int y = 0; y < 7; y++)
                {
                    spaceCueControllers[x, y].ShowCanTarget(predicate((x, y)));
                }
            }
        }
    }
}