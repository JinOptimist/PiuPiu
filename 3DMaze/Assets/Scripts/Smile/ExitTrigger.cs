using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitTrigger : MonoBehaviour
{
    private TextMeshProUGUI scoreText;
    private const string SimplifiedMaze = "SimplifiedMaze";
    public Vector3 cameraMargin = new Vector3(-12, 12, -12);
    public int animationDuration = 700;

    private bool isActiveAnimation = false;

    private void Start()
    {
        scoreText = GameObject
            .Find("/PlayerLayout/Score Panel/Score")
            .GetComponent<TextMeshProUGUI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isActiveAnimation)
        {
            isActiveAnimation = true;
            UpdateScore();

            ShowPathAndRenturnToTheMainMenuAsync();
        }
    }

    private void UpdateScore()
    {
        var countOfLevels = GameObject
                .Find("/Maze")
                .transform
                .childCount;

        var score = PlayerPrefs.GetInt("Score");
        score += 100 * countOfLevels;
        PlayerPrefs.SetInt("Score", score);
        scoreText.text = score.ToString();
    }

    private async void ShowPathAndRenturnToTheMainMenuAsync()
    {
        MoveCameraToLookAtBuildingOutside();
        AllWallAreTransparent();
        AllRoomTriggersAreVisible();
        HighlightPath();

        await Task.Delay(PlayerPathStore.Path.Count * animationDuration);

        Debug.Log("Exit!");
        SceneManager.LoadScene("StartMenu");
    }

    
    private void MoveCameraToLookAtBuildingOutside()
    {
        var playerFollowCamera = GameObject.Find("PlayerFollowCamera");
        if (playerFollowCamera != null)
        {
            playerFollowCamera.SetActive(false);
        }

        var mainCamera = GameObject.Find("MainCamera");
        mainCamera.transform.position = new Vector3(-6, 66, -40);
        mainCamera.transform.rotation = new Quaternion(0, 0, 0, 0);
        //mainCamera.transform.Rotate(new Vector3(30, 45, 0));
        mainCamera.transform.Rotate(new Vector3(42, 37, 0));

        mainCamera
            .GetComponent<Camera>()
            .cullingMask = LayerMask.GetMask(SimplifiedMaze);

    }

    private void AllWallAreTransparent()
    {
        var fullTransperentMaterialPath = "Materials/ShowPath/FullTransperent";
        var fullTransperentMaterial = Resources.Load<Material>(fullTransperentMaterialPath);

        var walls = GameObject.FindGameObjectsWithTag("Wall");
        foreach (var wall in walls)
        {
            wall.GetComponent<Renderer>().material = fullTransperentMaterial;
        }
    }

    private void AllRoomTriggersAreVisible()
    {
        var defaultMaterialPath = "Materials/ShowPath/Default";
        var defaultMaterial = Resources.Load<Material>(defaultMaterialPath);

        var roomTrigers = GameObject.FindGameObjectsWithTag("TriggerRoom");
        foreach (var trigger in roomTrigers)
        {
            trigger.transform.localScale = new Vector3(2, 2, 2);

            var render = trigger.GetComponent<MeshRenderer>();
            render.enabled = true;
            render.GetComponent<Renderer>().material = defaultMaterial;
        }
    }

    private async void HighlightPath()
    {
        var visitedMaterialPath = "Materials/ShowPath/Visited";
        var visitedMaterial = Resources.Load<Material>(visitedMaterialPath);

        var mainCamera = GameObject.Find("MainCamera");
        for (int i = 0; i < PlayerPathStore.Path.Count; i++)
        {
            var room = PlayerPathStore.Path[i];

            room.GetComponent<Renderer>().material = visitedMaterial;

            var roomPosition = room.transform.position;
            var targetPosition = roomPosition + cameraMargin;

            StartCoroutine(MoveCamera(
                mainCamera,
                targetPosition,
                animationDuration / 1000));

            await Task.Delay(animationDuration);
        }
    }

    IEnumerator MoveCamera(GameObject camera, Vector3 targetPosition, float duration)
    {
        float elapsedTime = 0;
        var startPosition = camera.transform.position;
        while (elapsedTime < duration)
        {
            camera.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        camera.transform.position = targetPosition;
    }
}
