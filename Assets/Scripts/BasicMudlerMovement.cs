using UnityEngine;

public class BasicMudlerMovement : MonoBehaviour
{
    public float speed = 3f; //velocidad de movimiento del Mudler
    private int direction = 1; //direcci�n inicial (1 = adelante, -1 = atr�s)
    public int health = 3; //vida del enemigo
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // Para que no sea afectado por fuerzas externas
    }

    private void Update()
    {
        //mueve al enemigo en el eje Z
        transform.position += new Vector3(0, 0, direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Si choca con una pared, cambia de direcci�n
        if (other.CompareTag("Wall"))
        {
            direction *= -1;
        }

        // Si es un disparo de Milton, recibe da�o
        if (other.CompareTag("Projectile"))
        {
            TakeDamage();
            Destroy(other.gameObject); // Destruye la bala
        }

        // Si choca con Milton, puede hacer da�o o empujarlo (seg�n lo que prefieras)
        if (other.CompareTag("Player"))
        {
            other.GetComponent<MiltonLogic>().TakeDamage(transform.position);
        }
    }

    void TakeDamage()
    {
        health--;

        if (health <= 0)
        {
            Destroy(gameObject); // Elimina al enemigo si su vida llega a 0
        }
    }
}
