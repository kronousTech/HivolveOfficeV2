using UnityEngine;

namespace Common
{
    public enum SideToAdjust
    {
        Width,
        Height
    }

    public class RO3Image : MonoBehaviour
    {
        public float originalWidth;
        public float originalHeight;

        public SideToAdjust sideToAdjust;

        private void Awake()
        {
            Canvas.ForceUpdateCanvases();

            var t = GetComponent<RectTransform>();
            float newLength;
            switch (sideToAdjust)
            {
                case SideToAdjust.Width:
                    newLength = (t.rect.height * originalWidth) / originalHeight;
                    newLength *= 1.5f;
                    t.sizeDelta = new Vector2(newLength, 0);
                    //sideLength = (t.anchorMax.x - t.anchorMin.x) * canvasWidth;
                    break;
                case SideToAdjust.Height:
                    newLength = (t.rect.width * originalHeight) / originalWidth;
                    //t.anchorMin = new Vector2(t.anchorMin.x, 0.5f);
                    //t.anchorMax = new Vector2(t.anchorMax.x, 0.5f);
                    t.sizeDelta = new Vector2(0, newLength);
                    break;
            }
            
            //var newYpivot = t.anchorMin.y + (t.anchorMax.y - t.anchorMin.y) / 2;
            //t.anchorMin = new Vector2(0.5f, newYpivot);
            //t.anchorMax = new Vector2(0.5f, t.anchorMin.y);
            //
            //var nRect = new Rect(0, 0, imageNewWidth, imageHeight);
            //t.sizeDelta = new Vector2(nRect.width, nRect.height);
        }
    }
}
//var viewHeight = transform.parent.parent.GetComponent<PopulateProductInfo>().scrollView.rect.height;
//var panelHeight = (p.anchorMax.y - p.anchorMin.y) * viewHeight;
//var imageHeight = (t.anchorMax.y - t.anchorMin.y) * viewHeight;
