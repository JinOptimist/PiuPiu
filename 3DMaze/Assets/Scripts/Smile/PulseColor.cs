using UnityEngine;

public class PulseColor : MonoBehaviour
{
    public Color startColor = Color.red; // Начальный цвет
    public Color endColor = Color.blue;  // Конечный цвет
    public float speed = 1.0f;           // Скорость изменения цвета

    private Renderer cubeRenderer;

    void Start()
    {
        cubeRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        // Используем Mathf.PingPong для получения значения от 0 до 1, меняющегося со временем
        float t = Mathf.PingPong(Time.time * speed, 1.0f);

        // Интерполируем между начальным и конечным цветом
        cubeRenderer.material.color = Color.Lerp(startColor, endColor, t);
    }
}
