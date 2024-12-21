using UnityEngine;
using UnityEngine.UI;

public class BackpackTabs : MonoBehaviour
{
    [Header("Labels (Buttons)")]
    [SerializeField] private Button itemsLabel;
    [SerializeField] private Button skillsLabel;

    [Header("Content Panes")]
    [SerializeField] private GameObject itemsPane;
    [SerializeField] private GameObject skillsPane;

    [Header("Visual Feedback (Optional)")]
    [SerializeField] private Color selectedColor = Color.white;
    [SerializeField] private Color unselectedColor = new Color(0.7f, 0.7f, 0.7f);

    private void Start()
    {
        // Add click listeners
        itemsLabel.onClick.AddListener(() => ShowTab(true));
        skillsLabel.onClick.AddListener(() => ShowTab(false));

        // Initialize with items tab
        ShowTab(true);
    }

    private void ShowTab(bool showItems)
    {
        // Toggle pane visibility
        itemsPane.SetActive(showItems);
        skillsPane.SetActive(!showItems);

        // Update button colors (optional)
        if (itemsLabel.targetGraphic is Image itemImage)
        {
            itemImage.color = showItems ? selectedColor : unselectedColor;
        }
        if (skillsLabel.targetGraphic is Image skillImage)
        {
            skillImage.color = showItems ? unselectedColor : selectedColor;
        }
    }
}
