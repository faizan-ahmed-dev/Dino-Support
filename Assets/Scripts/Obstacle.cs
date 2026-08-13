using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float speed = 6f;
    private float killX = -12f;

    void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;

        if (transform.position.x < killX)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Dino"))
        {
            DinoRunManager.Instance.OnDinoHit();
        }
    }
}