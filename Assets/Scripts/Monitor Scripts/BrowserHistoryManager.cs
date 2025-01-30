using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class BrowserHistoryManager : MonoBehaviour
{
    [SerializeField]
    // Stack to keep track of panel history
    private Stack<GameObject> history = new Stack<GameObject>();

    [SerializeField]
    // Reference to the currently active panel
    private GameObject currentPanel;

    
    [SerializeField]
    private Button backButton;

    void Start()
    {
        // Initialize back button state
        if (backButton != null)
        {
            backButton.interactable = false;
        }
    }

    // Call this when navigating to a new panel
    public void NavigateToPanel(GameObject newPanel)
    {
        // If we have a current panel, add it to history and disable it
        if (currentPanel != null)
        {
            history.Push(currentPanel);
            print(history.Count);
            currentPanel.SetActive(false);
        }

        // Enable the new panel and set it as current
        newPanel.SetActive(true);
        currentPanel = newPanel;

        // Enable back button since we now have history
        if (backButton != null)
        {
            backButton.interactable = history.Count > 0;
        }
    }

    // Call this when the back button is pressed
    public void GoBack()
    {
        if (history.Count > 0)
        {
            // Disable current panel
            if (currentPanel != null)
            {
                currentPanel.SetActive(false);
            }

            // Get and enable the previous panel
            print(history.Count);
            currentPanel = history.Pop();
            currentPanel.SetActive(true);

            // Update back button state
            if (backButton != null)
            {
                backButton.interactable = history.Count > 0;
            }
        }
    }

    // Optional: Clear history when closing the browser
    public void ClearHistory()
    {
        history.Clear();
        if (backButton != null)
        {
            backButton.interactable = false;
        }
    }
}