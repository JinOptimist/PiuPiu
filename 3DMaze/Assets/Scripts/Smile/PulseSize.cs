using UnityEngine;

public class PulseSize : MonoBehaviour
{
    public float min = .5f;
    public float max = 1f;
    public float speed = 1.0f;

    private Transform cubeTransform;
    private Vector3 initialScale;
    private Vector3 minScale;

    void Start()
    {
        cubeTransform = GetComponent<Transform>();
        initialScale = cubeTransform.transform.localScale;
        minScale = initialScale * min;
    }

    void Update()
    {
        // »спользуем Mathf.PingPong дл€ получени€ значени€ от 0 до 1, мен€ющегос€ со временем
        float t = Mathf.PingPong(Time.time * speed, 1.0f);

        // »нтерполируем между начальным и конечным цветом
        cubeTransform.transform.localScale = Vector3.Lerp(initialScale, minScale, t);
    }
}
