using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSwapper : MonoBehaviour
{
    public enum Scenes
    {
        MainScene = 1,
        GameOver = 2,
        MainMenu = 3
    }

    public Scenes dropDown;
    public void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public void SwitchScenes()
    {
        string tmp = "";
        switch (dropDown)
        {
            case Scenes.MainScene:
                tmp = "MainScene";
                break;
            case Scenes.GameOver:
                tmp = "GameOver";
                break;
            case Scenes.MainMenu:
                tmp = "MainMenu";
                break;
        }
        SceneManager.LoadScene(tmp);
    }
}
