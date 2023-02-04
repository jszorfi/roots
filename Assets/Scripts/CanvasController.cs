using UnityEngine;

public class CanvasController : MonoBehaviour
{
    private CanvasGroup potatoSkills;
    private CanvasGroup carrotSkills;
    private CanvasGroup radishSkills;
    private CanvasGroup builderSkills;

    private CanvasGroup displayedSkills;
    private void Start()
    {
        potatoSkills = gameObject.transform.GetChild(0).gameObject.GetComponent<CanvasGroup>();
        carrotSkills = gameObject.transform.GetChild(0).gameObject.GetComponent<CanvasGroup>();
        radishSkills = gameObject.transform.GetChild(0).gameObject.GetComponent<CanvasGroup>();
        builderSkills = gameObject.transform.GetChild(0).gameObject.GetComponent<CanvasGroup>();
    }

    private void ChangeDisplayedSkills(CanvasGroup skillsToDisplay)
    {
        if (displayedSkills != null)
            displayedSkills.alpha = 0;
        skillsToDisplay.alpha = 1;
        displayedSkills = skillsToDisplay;
    }

    public void DisplayPotatoSkills()
    {
        ChangeDisplayedSkills(potatoSkills);
    }
    public void DisplayCarrotSkills()
    {
        ChangeDisplayedSkills(carrotSkills);
    }
    public void DisplayRadishSkills()
    {
        ChangeDisplayedSkills(radishSkills);
    }
    public void DisplayBuilderSkills()
    {
        ChangeDisplayedSkills(builderSkills);
    }
}
