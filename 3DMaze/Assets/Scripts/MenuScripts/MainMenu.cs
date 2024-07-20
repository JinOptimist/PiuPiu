using Assets.GameData;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartEasyGame()
    {
        MazeParameter.Length = 3;
        MazeParameter.Width = 3;
        MazeParameter.Height = 3;
        MazeParameter.Seed = 42;
        SceneManager.LoadScene("Playground");
    }

    public void StartMediumGame()
    {
        MazeParameter.Length = 5;
        MazeParameter.Width = 5;
        MazeParameter.Height = 5;
        SceneManager.LoadScene("Playground");
    }

    public void StartHardGame()
    {
        MazeParameter.Length = 10;
        MazeParameter.Width = 10;
        MazeParameter.Height = 10;
        SceneManager.LoadScene("Playground");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
