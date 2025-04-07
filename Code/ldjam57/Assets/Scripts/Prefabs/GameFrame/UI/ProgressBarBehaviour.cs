using UnityEngine;
using UnityEngine.UI;

public class ProgressBarBehaviour : MonoBehaviour
{
    [SerializeField]
    Image fillImage;
    [Range(0, 1)] public float fillAmount = 1f;

    public RectTransform barRectTransform;

    private void Update()
    {
        fillImage.fillAmount = fillAmount;
    }

    public void SetValue(float value)
    {
        fillAmount = Mathf.Clamp01(value);
    }

    public void setColor(Color color)
    {
        fillImage.color = color;
    }
}
