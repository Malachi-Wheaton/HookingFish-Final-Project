using UnityEngine;
using UnityEngine.SceneManagement;

public class Gamemanager : MonoBehaviour
{
    public GameUIManager uiManager;

    void Update()
    {
        CheckEnemiesRemaining();
    }

    void CheckEnemiesRemaining()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length == 0)
        {
            if (uiManager != null)
                uiManager.ShowWinScreen();
            else
                Debug.LogWarning("UI Manager not assigned in GameManager!");
        }
    }
}
