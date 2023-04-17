using UnityEngine;
using static KompasCore.UI.UIController;

namespace KompasCore.UI
{
    public class CardModelController : MonoBehaviour
    {
        [Header("Zoomed Out")]
        public GameObject zoomedOutAll;
        public GameObject zoomedOutChar;
        public GameObject zoomedOutNonChar;

        [Header("Zoomed In - No Effect Text")]
        public GameObject zoomedInNoTextAll;
        public GameObject zoomedInNoTextChar;
        public GameObject zoomedInNoTextNonChar;

        [Header("Zoomed In - With Effect Text")]
        public GameObject zoomedInWithTextAll;
        public GameObject zoomedInWithTextChar;
        public GameObject zoomedInWithTextNonChar;

        [Header("Zoom-agnostic arrays")]
        public MeshRenderer[] cardArtImages;

        /*[Tooltip("Every GameObject whose frame material needs to be updated (i.e. for friendly vs enemy color)")]
        public GameObject[] frameObjects;*/

        public void ShowZoom(ZoomLevel zoomLevel, bool isChar)
        {
            //Debug.Log($"Showing zoom level {zoomLevel}");
            zoomedOutAll.SetActive(zoomLevel == ZoomLevel.ZoomedOut);
            zoomedOutChar.SetActive(zoomLevel == ZoomLevel.ZoomedOut && isChar);
            zoomedOutNonChar.SetActive(zoomLevel == ZoomLevel.ZoomedOut && !isChar);

            zoomedInNoTextAll.SetActive(zoomLevel == ZoomLevel.ZoomedInNoEffectText);
            zoomedInNoTextChar.SetActive(zoomLevel == ZoomLevel.ZoomedInNoEffectText && isChar);
            zoomedInNoTextNonChar.SetActive(zoomLevel == ZoomLevel.ZoomedInNoEffectText && !isChar);

            zoomedInWithTextAll.SetActive(zoomLevel == ZoomLevel.ZoomedInWithEffectText);
            zoomedInWithTextChar.SetActive(zoomLevel == ZoomLevel.ZoomedInWithEffectText && isChar);
            zoomedInWithTextNonChar.SetActive(zoomLevel == ZoomLevel.ZoomedInWithEffectText && !isChar);
        }

        public void ShowImage(Texture cardImageTexture)
        {
            foreach(var img in cardArtImages) img.material.mainTexture = cardImageTexture;
        }
    }
}