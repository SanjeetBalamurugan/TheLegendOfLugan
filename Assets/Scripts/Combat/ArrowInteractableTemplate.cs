using UnityEngine;

public class ArrowInteractableTemplate : MonoBehaviour, IArrowInteractable
{
    [SerializeField] private string objectName = "Interactive Object";

    public void OnArrowHit(TPVPlayerCombat.ArrowType arrowType)
    {
        switch (arrowType)
        {
            case TPVPlayerCombat.ArrowType.Physical:
                OnPhysicalArrow();
                break;

            case TPVPlayerCombat.ArrowType.Pyro:
                OnPyroArrow();
                break;

            case TPVPlayerCombat.ArrowType.Hydro:
                OnHydroArrow();
                break;

            // add more arrow types if you extend TPVPlayerCombat.ArrowType
            default:
                Debug.Log($"{objectName} hit by {arrowType}, but no behavior implemented.");
                break;
        }
    }

    private void OnPhysicalArrow()
    {
        Debug.Log($"{objectName} was hit by a Physical Arrow.");
        // TODO: Add your effect/logic here
    }

    private void OnPyroArrow()
    {
        Debug.Log($"{objectName} was hit by a Pyro Arrow.");
        // TODO: Burn, ignite VFX, trigger fire logic
    }

    private void OnHydroArrow()
    {
        Debug.Log($"{objectName} was hit by a Hydro Arrow.");
        // TODO: Splash effect, extinguish fire, spawn water particles
    }
}
