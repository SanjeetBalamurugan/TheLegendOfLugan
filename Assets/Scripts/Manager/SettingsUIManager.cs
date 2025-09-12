using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SettingsUIManager : MonoBehaviour
{
    [Header("Category Buttons Panel")]
    [SerializeField] private Transform categoryPanel; // Parent that holds all category buttons

    [Header("Category Menus")]
    [SerializeField] private GameObject audioMenuPrefab;
    [SerializeField] private GameObject graphicsMenuPrefab;
    [SerializeField] private GameObject licenseMenuPrefab;

    private Dictionary<string, GameObject> categoryMenus = new Dictionary<string, GameObject>();
    private GameObject currentActiveMenu;

    private void Awake()
    {
        // Map button names to prefabs
        categoryMenus.Add("Audio", audioMenuPrefab);
        categoryMenus.Add("Graphics", graphicsMenuPrefab);
        categoryMenus.Add("License", licenseMenuPrefab);

        // Bind buttons in category panel
        foreach (Button btn in categoryPanel.GetComponentsInChildren<Button>())
        {
            string categoryName = btn.name; // Button name must match dictionary key
            if (categoryMenus.ContainsKey(categoryName))
                btn.onClick.AddListener(() => OpenCategory(categoryName));
        }

        // Start with no menu shown
        HideAllMenus();
    }

    private void OpenCategory(string categoryName)
    {
        HideAllMenus();

        if (categoryMenus.TryGetValue(categoryName, out GameObject menuPrefab))
        {
            menuPrefab.SetActive(true);
            currentActiveMenu = menuPrefab;
        }
        else
        {
            Debug.LogWarning($"No menu found for category: {categoryName}");
        }
    }

    private void HideAllMenus()
    {
        foreach (var kvp in categoryMenus)
        {
            if (kvp.Value != null)
                kvp.Value.SetActive(false);
        }
        currentActiveMenu = null;
    }
}
