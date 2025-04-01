using UnityEngine;
using System.Collections;
using UnityEditor.Tilemaps;

public class BasicMudlerMovement : MonoBehaviour
{
    public float speed = 3f; //velocidad de movimiento del Mudler
    private int direction = 1; //dirección inicial (1 = adelante, -1 = atrás)
    public float radius = 5f; //radio del círculo (para mover a los Mudlers en círculo)
    private float angle = 0f; //ángulo para el movimiento circular
    public Vector3 center; //centro del círculo sobre el que se van a mover los Mudlers de movimiento circular
    public int health = 3; //vida del enemigo
    public float shrinkSpeed = 0.6f; //velocidad a la que el enemigo se encoge
    public float hurtDuration = 0.5f; //duración de la animación de daño
    public float dyingDuration = 10f; //duración de la animación de muerte
    private bool isDying = false; //para saber si el enemigo se está muriendo (y evitar que se mueva mientras se muere y que le podamos seguir golpeando)
    private Rigidbody rb;

    public GameObject coinPrefab; //prefab de la moneda
    public GameObject littleWaterBottlePrefab; //prefab de la botella de agua pequeña
    public GameObject keyPrefab; //prefab de la llave

    public Animator animator; //Animator para cambiar entre las distintas animaciones

    public static int enemiesDefeated = 0; //contador de Mudlers derrotados por Milton
    public int[] enemiesWithKey = { 1, 5, 10, 11 }; //números en los que los enemigos tienen que soltar la llave del nivel (en la primera sala el enemigo que hay, en la segunda los dos, en la 4 los 3 y en la última el Mudler especial)

    public enum MovementType { Horizontal, Vertical, Circular }; //tipo de movimiento
    public MovementType movementType = MovementType.Vertical; //por defecto se mueve verticalmente

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; //para que no sea afectado por fuerzas externas
        center = transform.position; //establece el centro del círculo (en el caso de los Mudlers de movimiento circular) como la posición inicial del Mudler
    }

    private void Update()
    {
        //si el enemigo NO se está muriendo, se mueve
        if (!isDying)
        {
            //mover dependiendo del tipo de movimiento
            switch (movementType)
            {
                case MovementType.Vertical:
                    transform.position += new Vector3(0, 0, direction * speed * Time.deltaTime);
                    break;

                case MovementType.Horizontal:
                    transform.position += new Vector3(direction * speed * Time.deltaTime, 0, 0); 
                    break;

                case MovementType.Circular:
                    MoveInCircle(); 
                    break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //si choca con una pared, cambia de dirección
        if (other.CompareTag("Wall"))
        {
            direction *= -1; //cambia la dirección

            if (movementType == MovementType.Horizontal)
            {
                Flip();
            }

        }

        //si choca contra cualquier objeto (llave, moneda o botella de agua) ignora la colisión para no arrastrar por la pantalla los objetos
        if (other.CompareTag("Key") || other.CompareTag("Coin") || other.CompareTag("LittleWaterBottle"))
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), other);
        }

        //si es un disparo de Milton, recibe daño
        if (other.CompareTag("Projectile"))
        {
            TakeDamage();
            Destroy(other.gameObject); //destruye la bala al ser recibida
        }

        //si choca con Milton y no está muriéndose, le hace daño (método en el Script de Milton)
        if (other.CompareTag("Player") && !isDying)
        {
            other.GetComponent<MiltonLogic>().TakeDamage(transform.position);
        }
    }

    //método para el movimiento circular
    private void MoveInCircle()
    {
        //usar la velocidad para determinar lo rápido que gira el Mudler en torno al centro de su círculo
        angle += speed * Time.deltaTime * 20f; //ajusta el factor "20f" para que el ángulo avance más rápido, si es necesario

        //mantiene el ángulo entre 0 y 360 grados
        if (angle >= 360f)
        {
            angle -= 360f;
        }

        //calcula las nuevas posiciones X y Z usando seno y coseno para formar el círculo
        float x = center.x + Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
        float z = center.z + Mathf.Sin(angle * Mathf.Deg2Rad) * radius;

        //mantiene la misma altura en Y
        float y = transform.position.y;

        //actualiza la posición del Mudler
        transform.position = new Vector3(x, y, z);
    }

    private void TakeDamage()
    {
        //si el enemigo NO se está muriendo, recibe daño y demás
        if (!isDying)
        {
            health--;

            animator.SetBool("isHurt", true);

            if (health <= 0)
            {
                isDying = true;

                //destruir el Collider antes de destruir el objeto
                Destroy(GetComponent<Collider>());

                //activar la animación de muerte
                animator.SetBool("isDying", true);

                //detener movimiento y demás comportamientos
                rb.linearVelocity = Vector3.zero;

                //iniciar el encogimiento hacia abajo
                StartCoroutine(ShrinkTowardsGround());

                //esperar la duración de la animación antes de destruir el objeto
                StartCoroutine(WaitForDeathAnimation());
                DropItems();
            }
            else
            {
                StartCoroutine(ResetHurtAnimation());
            }
        }
    }

    //método para girar a los enemigos que se mueven en horizontal
    private void Flip()
    {
        //cambia la escala en X del objeto para simular que el enemigo se gira
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;  //invierte la escala en el eje X para voltear el sprite
        transform.localScale = currentScale;  //asigna la nueva escala
    }

    IEnumerator ResetHurtAnimation()
    {
        //Espera el tiempo de la animación de daño
        yield return new WaitForSeconds(hurtDuration); // Ajusta este tiempo según la duración de la animación de daño

        //Después de la animación de daño, desactivar el parámetro 'isHurt'
        animator.SetBool("isHurt", false);
    }

    IEnumerator ShrinkTowardsGround()
    {
        Vector3 initialScale = transform.localScale;

        //desactivar la gravedad mientras se encoge para que no flote hacia arriba
        rb.useGravity = false;

        //reduce la escala en Y hasta llegar a 0 (o a un valor pequeño para simular el charco de barro)
        while (transform.localScale.y > 0.1f)
        {
            transform.localScale = new Vector3(initialScale.x, transform.localScale.y - shrinkSpeed * Time.deltaTime, initialScale.z);
            yield return null;
        }

        transform.localScale = new Vector3(initialScale.x, 0, initialScale.z); //para que se quede completamente aplastado

        rb.useGravity = true; //volver a activar la gravedad
    }

    IEnumerator WaitForDeathAnimation()
    {
        //espera el tiempo de duración de la animación de muerte
        yield return new WaitForSeconds(dyingDuration); // Ajusta este tiempo según la duración de la animación de muerte

        //después de la animación, destruir al enemigo
        Destroy(gameObject);
    }

    private void DropItems()
    {
        enemiesDefeated++; //aumenta el contador de enemigos derrotados
        Debug.Log("ENEMIGOS DERROTADOS: " +  enemiesDefeated);
        bool hasKey = System.Array.Exists(enemiesWithKey, element => element == enemiesDefeated);

        if (hasKey)
        {
            //si el enemigo es el último, el que tiene la llave de la sala, deja caer la llave y 2 objetos aleatorios
            SpawnObject(keyPrefab, transform.position + Vector3.up * 0.5f);
            SpawnObject(GetRandomItem(), transform.position + Vector3.right * 0.5f);
            SpawnObject(GetRandomItem(), transform.position + Vector3.left * 0.5f);
        }
        else
        {
            //si el enemigo no es el último de la sala, deja caer 3 objetos aleatorios
            SpawnObject(GetRandomItem(), transform.position + Vector3.forward * 0.5f);
            SpawnObject(GetRandomItem(), transform.position + Vector3.back * 0.5f);
            SpawnObject(GetRandomItem(), transform.position + Vector3.right * 0.5f);
        }
    }

    //método con iuna ternaria para, con un valor aleatorio, si es entre 0.5 y 1 instancie un prefab de la moneda, si es entre 0.5 y 0, una botella de agua
    private GameObject GetRandomItem()
    {
        return Random.value > 0.5f ? coinPrefab : littleWaterBottlePrefab;
    }

    //método para dejar caer los objetos cuando el enemigo muere, recibiendo el objeto que va a dejar caer y la dirección hacia donde lo va a dejar caer
    private void SpawnObject(GameObject prefab, Vector3 position)
    {
        GameObject obj = Instantiate(prefab, position, Quaternion.identity);
        AddJumpEffect(obj);
    }

    private void AddJumpEffect(GameObject obj)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();

        //aplicamos una fuerza en una dirección aleatoria para que rebote con más gracia
        Vector3 jumpDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(3f, 6f), Random.Range(-1f, 1f));
        rb.AddForce(jumpDirection, ForceMode.Impulse);
    }
}
