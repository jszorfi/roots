using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour
{
    public void NewGame()
    {
        SceneManager.LoadScene("MasterScene");
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
