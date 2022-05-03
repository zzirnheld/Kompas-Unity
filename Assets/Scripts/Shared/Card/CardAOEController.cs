using UnityEngine;

namespace KompasCore.Cards
{
    public class CardAOEController : MonoBehaviour
    {
        public const float baseAoe = 0.2f;
        public const float aoeScaling = 2.8f;

        public CardController cardCtrl;

        public GameObject aoeBlock;

        public void Show(int aoe)
        {
            aoeBlock.transform.localScale = new Vector3(baseAoe + (aoeScaling * aoe), 0.01f, baseAoe + (aoeScaling * aoe));
            aoeBlock.SetActive(true);
        }

        public void Hide()
        {
            aoeBlock.SetActive(false);
        }
    }
}