using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartScript : MonoBehaviour
{
    public void RestartGame()
    {
        SceneManager.LoadScene("InstructionScene");
    }
}
