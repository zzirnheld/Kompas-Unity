using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KompasCore.Cards
{
    public class CardAOEController : MonoBehaviour
    {
        public CardController cardCtrl;

        public GameObject aoeBlock;
        public MeshRenderer aoeBlockRenderer;

        public void Show(int aoe)
        {
            aoeBlock.transform.localScale = new Vector3(1f + (4f * aoe), 0.01f, 1f + (4f * aoe));
            aoeBlock.SetActive(true);
        }

        public void Hide()
        {
            aoeBlock.SetActive(false);
        }
    }
}