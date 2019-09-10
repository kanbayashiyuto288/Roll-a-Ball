using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;


public class GameController : MonoBehaviour {

    public UnityEngine.UI.Text scoreLabel;
    public GameObject winnerLabelObject;

    public void Update()
    {
        int count = GameObject.FindGameObjectsWithTag("Item").Length;
        scoreLabel.text = count.ToString();
        if (Input.GetKey(KeyCode.Escape)) Quit();


        if (count == 0)
        {
            SceneManager.LoadScene("endscene");

 //           winnerLabelObject.SetActive(true);
        }
    }
    void Quit()
    {
 //       UnityEditor.EditorApplication.isPlaying = false;
        UnityEngine.Application.Quit();
    }

}
