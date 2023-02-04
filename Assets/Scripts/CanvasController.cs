using UnityEngine;

public class CanvasController : MonoBehaviour
{
    private CanvasGroup potatoSkills;
    private void Start()
    {
        potatoSkills = gameObject.transform.GetChild(0).gameObject.GetComponent<CanvasGroup>();
    }

    public void DisplayPotatoSkills()
    {
        if (potatoSkills.alpha == 1)
            potatoSkills.alpha = 0;
        else if (potatoSkills.alpha == 0)
            potatoSkills.alpha = 1;
    }

}
