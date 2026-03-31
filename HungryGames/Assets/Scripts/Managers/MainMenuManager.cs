using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void Quit()
    {
        if(Application.isEditor) UnityEditor.EditorApplication.isPlaying = false;
        else Application.Quit();
    }
}
