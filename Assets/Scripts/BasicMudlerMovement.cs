using UnityEngine;

public class BasicMudlerMovement : MonoBehaviour
{
    public float speed = 3f; //velocidad de movimiento del Mudler
    private int direction = 1; //dirección inicial (1 = adelante, -1 = atrás)
    public int health = 3; //vida del enemigo
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; //para que no sea afectado por fuerzas externas
    }

    private void Update()
    {
        //mueve al enemigo en el eje Z
        transform.position += new Vector3(0, 0, direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        //si choca con una pared, cambia de dirección
        if (other.CompareTag("Wall"))
        {
            direction *= -1;
        }

        //si es un disparo de Milton, recibe daño
        if (other.CompareTag("Projectile"))
        {
            TakeDamage();
            Destroy(other.gameObject); //destruye la bala al ser recibida
        }

        //si choca con Milton, le hace daño (método en el Script de Milton)
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
            Destroy(gameObject); //elimina al enemigo si su vida llega a 0
        }
    }
}
