using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ProgressBar : MonoBehaviour
{
    public int minimum = 0;
    public int maximum = 100;
    public int current = 0;

    public Image mask;

    void Start()
    {
        UpdateProgressBar();
    }

    void Update()
    {
        UpdateProgressBar();
    }

    void UpdateProgressBar()
    {
        current = Mathf.Clamp(current, minimum, maximum);

        float fillAmount = (float)(current - minimum) / (maximum - minimum);

        if (mask != null)
        {
            mask.fillAmount = fillAmount;
        }
    }

#if UNITY_EDITOR
    [MenuItem("GameObject/UI/Linear Progress Bar")]
    public static void AddLinearProgressBar()
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>("UI/Linear Progress Bar"));
        obj.transform.SetParent(Selection.activeGameObject.transform, false);
    }
#endif
}
