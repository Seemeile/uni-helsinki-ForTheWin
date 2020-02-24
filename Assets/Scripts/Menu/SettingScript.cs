using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Entities;
using Unity.Collections;

public class SettingScript : MonoBehaviour
{
    public static bool newGame = true;
    EntityManager entityManager;
    public static NativeArray<Entity> listOfEntities;

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
        GameHandler.hasToDelete = true;
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1;
    }
    public void Save()
    {
        SaveSystem.listOfPreviousStructures.Clear();
        SaveSystem.listOfPreviousUnits.Clear();
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1;
    }
    public void Load()
    {
        newGame = false;
        GameHandler.hasToLoad = true;
        SceneManager.LoadScene("test");
    }
    public void NewGame()
    {
        SceneManager.LoadScene("test");
        newGame = true;
    }
}
