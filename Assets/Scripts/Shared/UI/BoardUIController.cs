﻿using KompasCore.Cards;
using KompasCore.GameCore;
using System;
using UnityEngine;

namespace KompasCore.UI
{
    public abstract class BoardUIController : MonoBehaviour
    {
        public const float BoardLenOffset = 7f;
        public const float LenOneSpace = 2f;
        public const float SpaceOffset = LenOneSpace / 2f;
        public const float CardHeight = 0.15f;

        public abstract BoardController BoardController { get; }
        public abstract UIController UIController { get; }

        public GameObject spaceCueControllerPrefab;
        public Transform spaceCueCubesParent;

        private readonly SpaceCueController[,] spaceCueControllers = new SpaceCueController[Space.BoardLen, Space.BoardLen];
        private GameCardBase currShowingFor;

        private static Vector3 GridIndicesToCuePos(int x, int y)
        {
            return new Vector3(GridIndexToPos(x), 0.01f, GridIndexToPos(y));
        }

        public static int PosToGridIndex(float pos)
            => (int)((pos + BoardLenOffset) / (LenOneSpace));

        public static float GridIndexToPos(int gridIndex)
            => (float)((gridIndex * LenOneSpace) + SpaceOffset - BoardLenOffset);

        public static Vector3 GridIndicesToCardPos(int x, int y)
            => new Vector3(GridIndexToPos(x), CardHeight, GridIndexToPos(y));

        private void Awake()
        {
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    GameObject cue = Instantiate(spaceCueControllerPrefab, spaceCueCubesParent);
                    cue.transform.localPosition = GridIndicesToCuePos(i, j);
                    spaceCueControllers[i, j] = cue.GetComponent<SpaceCueController>();
                    spaceCueControllers[i, j].Init(this, (i, j));
                }
            }
        }

        public virtual void OnMouseDown()
        {
            //select nothing
            UIController.CardViewController.Show(null);
        }

        public void ShowForCard(GameCardBase card)
        {
            currShowingFor = card;

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    var cue = spaceCueControllers[i, j];

                    if (card.MovementRestriction.IsValidNormalMove((i, j)))
                        cue.Show(SpaceCueController.CueType.Move);
                    else if (card.MovementRestriction.WouldBeValidNormalMove((i, j)))
                        cue.Show(SpaceCueController.CueType.MoveOpenGamestate);
                    else if (card.AttackRestriction.IsValidAttack(BoardController.GetCardAt((i, j)), stackSrc: null))
                        cue.Show(SpaceCueController.CueType.Attack);
                    else if (card.PlayRestriction.IsRecommendedNormalPlay((i, j), card.Controller))
                        cue.Show(SpaceCueController.CueType.Play);
                    else
                        cue.Clear();
                }
            }
        }

        public void RefreshShownCard() => ShowForCard(currShowingFor);

        public void ShowNothing()
        {
            foreach (var cue in spaceCueControllers) cue.Clear();
        }

        public void ShowSpaceTargets(Func<(int, int), bool> predicate)
        {
            for (int x = 0; x < 7; x++)
            {
                for (int y = 0; y < 7; y++)
                {
                    spaceCueControllers[x, y].Show(SpaceCueController.CueType.Target, predicate((x, y)));
                }
            }
        }

        public abstract void Clicked(Space position, GameCard focusCardOverride = null);
    }
}