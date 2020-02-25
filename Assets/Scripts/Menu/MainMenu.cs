using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Entities;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        purgeActiveWorld();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Game");
    }
    
    public void Help()
    {
        SceneManager.LoadScene("Help");
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void purgeActiveWorld()
    {
        EntityManager entityManager = World.Active.EntityManager;
        foreach(Entity entity in entityManager.GetAllEntities())
        {
            entityManager.DestroyEntity(entity);   
        }
    }
}
