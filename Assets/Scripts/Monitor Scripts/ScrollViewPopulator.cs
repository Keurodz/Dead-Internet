using UnityEngine;
using UnityEngine.UI;

public class ScrollViewPopulator : MonoBehaviour
{
    public GameObject itemPrefab; // The prefab of the item (button, text, etc.)
    public Transform contentPanel; // The content panel of the ScrollView
    public int numberOfItems = 10; // Number of items to populate in the ScrollView

    void Start()
    {
        PopulateScrollView();
    }

    // Populate the ScrollView with content
    void PopulateScrollView()
    {
        // Clear any existing items
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        // Instantiate new items
        for (int i = 0; i < numberOfItems; i++)
        {
            // Instantiate the item prefab
            GameObject newItem = Instantiate(itemPrefab, contentPanel);

            Text itemText = newItem.GetComponentInChildren<Text>();
            if (itemText != null)
            {
                itemText.text = "Item " + (i + 1); // Set the text dynamically
            }
        }
    }
}
