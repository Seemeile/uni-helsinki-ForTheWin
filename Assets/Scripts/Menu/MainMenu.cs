using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Entities;
using Unity.Entities.Serialization;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        purgeActiveWorld();
        SceneManager.LoadScene("test");
    }
    
    public void LoadGame()
    {
        purgeActiveWorld();
        EntityManager entityManager = World.Active.EntityManager;
        using (Unity.Entities.Serialization.BinaryReader reader = new StreamBinaryReader("savegame.ftw"))
        {
            using (World tempWorld = new World("loading"))
            {
                EntityManager e2 = tempWorld.EntityManager;
                SerializeUtility.DeserializeWorld(e2.BeginExclusiveEntityTransaction(), reader, 1);
                e2.EndExclusiveEntityTransaction();
                entityManager.MoveEntitiesFrom(e2);
            }
        }
        SceneManager.LoadScene("test");
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
