using UnityEngine;
using UnityEngine.SceneManagement;

public class Gamemanager : MonoBehaviour
{
    void Update()
    {
        CheckEnemiesRemaining();
    }

    void CheckEnemiesRemaining()
    {
        // Sahnedeki tüm "Enemy" tagli nesneleri say
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length == 0)
        {
            LoadNextScene();
        }
    }

    void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Sonraki sahne varsa yükle
        if (currentSceneIndex + 1 < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(currentSceneIndex + 1);
        }
        else
        {
            Debug.Log("Son sahne, sahne değişimi yok.");
        }
    }
}
