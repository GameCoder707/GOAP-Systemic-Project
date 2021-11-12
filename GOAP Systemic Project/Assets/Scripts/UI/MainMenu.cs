using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void EnterScenario1()
    {
        SceneManager.LoadScene("Scenario 1");
    }
    public void EnterScenario2()
    {
        SceneManager.LoadScene("Scenario 2");
    }
    public void EnterScenario3()
    {
        SceneManager.LoadScene("Scenario 3");
    }
    public void EnterScenario4()
    {
        SceneManager.LoadScene("Scenario 4");
    }
    public void EnterScenario5()
    {
        SceneManager.LoadScene("Scenario 5");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
