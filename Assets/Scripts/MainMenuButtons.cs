using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenuButtons : MonoBehaviour
{
    public void JoinServerButton()
    {
        SceneManager.LoadScene(1);
    }
    public void HostServerButton()
    {
        SceneManager.LoadScene(2);
    }
    public void QuitButton()
    {
        Application.Quit();
    }
}