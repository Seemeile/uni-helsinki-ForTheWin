using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Entities;
using Unity.Entities.Serialization;
using UnityEngine.Serialization;
using Unity.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SettingScript : MonoBehaviour
{
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
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }

    public void Save()
    {
        EntityManager entityManager = World.Active.EntityManager;
        using (Unity.Entities.Serialization.BinaryWriter writer = new StreamBinaryWriter("savegame.ftw"))
        {
            int[] sharedComponents;
            SerializeUtility.SerializeWorld(entityManager, writer, out sharedComponents);
        }
        Time.timeScale = 1;
    }
}
