using UnityEngine;

namespace KompasCore.UI
{
    public class SpaceCueController : MonoBehaviour
    {
        public GameObject canMoveCube;
        public GameObject canAttackCube;
        public GameObject canPlayCube;

        public GameObject canTargetCube;

        public void ShowCanMove()
        {
            canMoveCube.SetActive(true);
            canAttackCube.SetActive(false);
            canPlayCube.SetActive(false);
        }

        public void ShowCanAttack()
        {
            canMoveCube.SetActive(false);
            canAttackCube.SetActive(true);
            canPlayCube.SetActive(false);
        }

        public void ShowCanPlay()
        {
            canMoveCube.SetActive(false);
            canAttackCube.SetActive(false);
            canPlayCube.SetActive(true);
        }

        public void ShowCanNone()
        {
            canMoveCube.SetActive(false);
            canAttackCube.SetActive(false);
            canPlayCube.SetActive(false);
        }

        public void ShowCanTarget(bool can = true) => canTargetCube.gameObject.SetActive(can);
    }
}