using UnityEngine;

public class BarScaler : MonoBehaviour
{
    [SerializeField] private float maxValue = 100f;
    [SerializeField] private float minValue = 0f;
    
    private RectTransform rectTransform;
    private float originalWidth;
    private bool isInitialized = false;

    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (!isInitialized)
        {
            rectTransform = GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                originalWidth = rectTransform.rect.width;
                isInitialized = true;
            }
        }
    }

    public void SetValue(float currentValue)
    {
        if (!isInitialized)
        {
            Initialize();
        }

        if (rectTransform != null)
        {
            currentValue = Mathf.Clamp(currentValue, minValue, maxValue);
            float scaleFactor = currentValue / maxValue;
            
            // Only modify the width while keeping all other properties unchanged
            Vector2 sizeDelta = rectTransform.sizeDelta;
            sizeDelta.x = originalWidth * scaleFactor;
            rectTransform.sizeDelta = sizeDelta;
        }
    }

    public void SetMaxValue(float value)
    {
        maxValue = value;
    }
}
