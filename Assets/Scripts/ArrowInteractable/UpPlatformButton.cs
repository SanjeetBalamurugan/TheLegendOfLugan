using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UpPlatformButton : MonoBehaviour, IArrowInteractable
{
    [SerializeField] private string objectName = "Interactive Object";
    [SerializeField] private Transform finalPlacement;
    [SerializeField] private List<GameObject> platformObjects;
    [SerializeField] private float moveSmoothness = 1f;

    public void OnArrowHit(TPVPlayerCombat.ArrowType arrowType)
    {
        foreach (GameObject platform in platformObjects)
        {
            platform.transform.Translate(platform.transform.position.x, finalPlacement.y, platform.transform.position.z);
        }
    }
}
