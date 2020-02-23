using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Entities;
using Unity.Collections;

public class SettingScript : MonoBehaviour
{
    public static bool isSaved = false;
    public void Pause()
    {
        Time.timeScale = 0;
    }
    public void Continue()
    {
        Time.timeScale = 1;
    }
    public void Quit()
    {
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1;
        isSaved = false;
    }
    public void Save()
    {
        isSaved = true;
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1;
    }
    public void Load()
    {
        SceneManager.LoadScene("test");
    }
    public void NewGame()
    {
        isSaved = false;
        SceneManager.LoadScene("test");
    }
}
