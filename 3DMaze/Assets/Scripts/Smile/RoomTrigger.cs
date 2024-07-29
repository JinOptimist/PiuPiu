using TMPro;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public string roomCoordinate;

    private TextMeshProUGUI roomText;
    private TextMeshProUGUI scoreText;

    private void Start()
    {
        roomText = GameObject
            .Find("/PlayerLayout/Room Panel/RoomNumberText")
            .GetComponent<TextMeshProUGUI>();

        scoreText = GameObject
            .Find("/PlayerLayout/Score Panel/Score")
            .GetComponent<TextMeshProUGUI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            roomText.text = roomCoordinate;

            var score = PlayerPrefs.GetInt("Score");
            score++;
            PlayerPrefs.SetInt("Score", score);
            scoreText.text = score.ToString();

            PlayerPathStore.Path.Add(gameObject);
        }
    }
}
