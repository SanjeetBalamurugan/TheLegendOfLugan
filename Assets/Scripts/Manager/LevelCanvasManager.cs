using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelCanvasManager : MonoBehaviour
{

    public Image elementImage;
    public TMP_Text elementText;

    public Sprite pyro;
    public Sprite hydro;
    public Sprite physical;

    public TPVPlayerCombat combat;

    // Update is called once per frame
    void Update()
    {
        if(combat.currentArrowType == TPVPlayerCombat.ArrowType.Pyro)
        {
            elementImage.sprite = pyro;
            elementText.text = "Pyro Arrow";
        } else if (combat.currentArrowType == TPVPlayerCombat.ArrowType.Hydro)
        {
            elementImage.sprite = hydro;
            elementText.text = "Hydro Arrow";

        }
        else if (combat.currentArrowType == TPVPlayerCombat.ArrowType.Physical)
        {
            elementImage.sprite = physical;
            elementText.text = "Physical Arrow";

        }
    }
}
