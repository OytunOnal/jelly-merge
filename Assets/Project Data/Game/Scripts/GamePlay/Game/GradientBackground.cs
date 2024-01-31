#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;

namespace JellyMerge
{
    public class GradientBackground : MonoBehaviour
    {
        [SerializeField]
        private RawImage backgroundImage;

        [Space]
        [SerializeField]
        private Color startColor = Color.white;
        [SerializeField]
        private Color endColor = Color.white;

        [Space]
        [SerializeField]
        private bool initOnAwake = false;

        private Texture2D backgroundTexture;

        private void Awake()
        {
            backgroundTexture = new Texture2D(1, 9);
            backgroundTexture.wrapMode = TextureWrapMode.Clamp;
            backgroundTexture.filterMode = FilterMode.Bilinear;

            if (initOnAwake)
                SetColor(startColor, endColor);
        }

        public void SetColor(Color color1, Color color2)
        {
            backgroundTexture.SetPixel(0, 0, color1);
            backgroundTexture.SetPixel(0, 1, Color.Lerp(color1, color2, 0.125f));
            backgroundTexture.SetPixel(0, 2, Color.Lerp(color1, color2, 0.250f));
            backgroundTexture.SetPixel(0, 3, Color.Lerp(color1, color2, 0.375f));
            backgroundTexture.SetPixel(0, 4, Color.Lerp(color1, color2, 0.500f));
            backgroundTexture.SetPixel(0, 5, Color.Lerp(color1, color2, 0.625f));
            backgroundTexture.SetPixel(0, 6, Color.Lerp(color1, color2, 0.750f));
            backgroundTexture.SetPixel(0, 7, Color.Lerp(color1, color2, 0.875f));
            backgroundTexture.SetPixel(0, 8, color2);

            backgroundTexture.Apply();
            backgroundImage.texture = backgroundTexture;
        }
    }
}