using UnityEngine;

namespace KompasCore.UI
{
    public class OscillatingController : MonoBehaviour
    {
        public float xFloatSpeedMult = 1f;
        public float yFloatSpeedMult = 1f;
        public float zFloatSpeedMult = 1f;
        public float xFloatAmplitude = 0f;
        public float yFloatAmplitude = 0f;
        public float zFloatAmplitude = 0f;
        public float xPosMod = 0f;
        public float yPosMod = 0f;
        public float zPosMod = 0f;

        public bool freeze = false;

        // Update is called once per frame
        void Update()
        {
            if (freeze) return;

            float x = xFloatAmplitude * Mathf.Sin(xFloatSpeedMult * Time.time) + xPosMod;
            float y = yFloatAmplitude * Mathf.Sin(yFloatSpeedMult * Time.time) + yPosMod;
            float z = zFloatAmplitude * Mathf.Sin(zFloatSpeedMult * Time.time) + zPosMod;

            transform.localPosition = new Vector3(x, y, z);
        }

        public void Enable(bool oscillate)
        {
            freeze = !oscillate;
            gameObject.SetActive(true);
        }

        public void Disable() => gameObject.SetActive(false);
    }
}