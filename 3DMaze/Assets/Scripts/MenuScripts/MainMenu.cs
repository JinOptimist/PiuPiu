using Assets.Scripts.Smile;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartEasyGame()
    {
        CreateDefaultPlayerPreference();

        PlayerPrefs.SetInt("Length", 3);
        PlayerPrefs.SetInt("Width", 3);
        PlayerPrefs.SetInt("Height", 3);
        PlayerPrefs.SetInt("Seed", 100);
        PlayerPrefs.SetInt("GenerationWeightsType", (int)GenerationWeightsType.GenericBuilding);

        SceneManager.LoadScene("Playground");
    }

    public void StartMediumGame()
    {
        CreateDefaultPlayerPreference();

        PlayerPrefs.SetInt("Length", 5);
        PlayerPrefs.SetInt("Width", 5);
        PlayerPrefs.SetInt("Height", 5);
        PlayerPrefs.DeleteKey("Seed");
        PlayerPrefs.SetInt("GenerationWeightsType", (int)GenerationWeightsType.FullRandom);

        SceneManager.LoadScene("Playground");
    }

    public void StartHardGame()
    {
        CreateDefaultPlayerPreference();

        PlayerPrefs.SetInt("Length", 8);
        PlayerPrefs.SetInt("Width", 5);
        PlayerPrefs.SetInt("Height", 15);
        PlayerPrefs.DeleteKey("Seed");
        PlayerPrefs.SetInt("GenerationWeightsType", (int)GenerationWeightsType.StairsEveryWhere);

        SceneManager.LoadScene("Playground");
    }

    private void CreateDefaultPlayerPreference()
    {
        if (!PlayerPrefs.HasKey("Score"))
        {
            PlayerPrefs.SetInt("Score", 0);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
