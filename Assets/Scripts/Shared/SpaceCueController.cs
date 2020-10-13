using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KompasCore.UI
{
    public class SpaceCueController : MonoBehaviour
    {
        public GameObject canMoveCube;
        public GameObject canAttackCube;
        public GameObject canTargetCube;

        public void ShowCanMove()
        {
            canMoveCube.SetActive(true);
            canAttackCube.SetActive(false);
        }

        public void ShowCanAttack()
        {
            canMoveCube.SetActive(false);
            canAttackCube.SetActive(true);
        }

        public void ShowCanNeither()
        {
            canMoveCube.SetActive(false);
            canAttackCube.SetActive(false);
        }

        public void ShowCanTarget(bool can = true) => canTargetCube.gameObject.SetActive(can);
    }
}