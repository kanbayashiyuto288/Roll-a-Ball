using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoad : MonoBehaviour
{

    public void StartGame1()
    {
        SceneManager.LoadScene("stage01");
    }

    public void StartGame2()
    {
        SceneManager.LoadScene("stage02");
    }

    public void StartGame3()
    {
        SceneManager.LoadScene("stage03");
    }

    public void EndGmae()
    {
        SceneManager.LoadScene("title");
    }

    public void EndGameall()
    {
        Application.Quit();
    }

}
