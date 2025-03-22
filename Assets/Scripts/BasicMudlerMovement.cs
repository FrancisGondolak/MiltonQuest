using UnityEngine;
using System.Collections;

public class BasicMudlerMovement : MonoBehaviour
{
    public float speed = 3f; //velocidad de movimiento del Mudler
    private int direction = 1; //direcci�n inicial (1 = adelante, -1 = atr�s)
    public int health = 3; //vida del enemigo
    public float shrinkSpeed = 0.6f; //velocidad a la que el enemigo se encoge
    public float hurtDuration = 0.5f; //duraci�n de la animaci�n de da�o
    public float dyingDuration = 10f; //duraci�n de la animaci�n de muerte
    private bool isDying = false; //para saber si el enemigo se est� muriendo (y evitar que se mueva mientras se muere y que le podamos seguir golpeando)
    private Rigidbody rb;

    public GameObject coinPrefab; //prefab de la moneda
    public GameObject littleWaterBottlePrefab; //prefab de la botella de agua peque�a
    public GameObject keyPrefab; //prefab de la llave

    public Animator animator; //Animator para cambiar entre las distintas animaciones

    public static int enemiesDefeated = 0; //contador de Mudlers derrotados por Milton
    public int[] enemiesWithKey = {1,3,6,7}; //n�meros en los que los enemigos tienen que soltar la llave del nivel (en la primera sala el enemigo que hay, en la segunda los dos, en la 4 los 3 y en la �ltima el Mudler especial)

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; //para que no sea afectado por fuerzas externas
    }

    private void Update()
    {
        //si el enemigo NO se est� muriendo, se mueve
        if (!isDying)
        {
            //mueve al enemigo en el eje Z
            transform.position += new Vector3(0, 0, direction * speed * Time.deltaTime);
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {

        //si choca con una pared, cambia de direcci�n
        if (other.CompareTag("Wall"))
        {
            direction *= -1;
        }

        //si es un disparo de Milton, recibe da�o
        if (other.CompareTag("Projectile"))
        {
            TakeDamage();
            Destroy(other.gameObject); //destruye la bala al ser recibida
        }

        //si choca con Milton y no est� muri�ndose, le hace da�o (m�todo en el Script de Milton)
        if (other.CompareTag("Player")  && !isDying)
        {
            other.GetComponent<MiltonLogic>().TakeDamage(transform.position);
        }
    }

    private void TakeDamage()
    {
        //si el enemigo NO se est� muriendo, recibe da�o y dem�s
        if (!isDying)
        {
            health--;

            animator.SetBool("isHurt", true);

            if (health <= 0)
            {
                isDying = true;

                //destruir el Collider antes de destruir el objeto
                Destroy(GetComponent<Collider>());

                //activar la animaci�n de muerte
                animator.SetBool("isDying", true);

                //detener movimiento y dem�s comportamientos
                rb.linearVelocity = Vector3.zero;

                //iniciar el encogimiento hacia abajo
                StartCoroutine(ShrinkTowardsGround());

                //esperar la duraci�n de la animaci�n antes de destruir el objeto
                StartCoroutine(WaitForDeathAnimation());
                DropItems();
            }
            else
            {
                StartCoroutine(ResetHurtAnimation());
            }
        }
    }

    IEnumerator ResetHurtAnimation()
    {
        // Espera el tiempo de la animaci�n de da�o
        yield return new WaitForSeconds(hurtDuration); // Ajusta este tiempo seg�n la duraci�n de la animaci�n de da�o

        // Despu�s de la animaci�n de da�o, desactivar el par�metro 'isHurt'
        animator.SetBool("isHurt", false);
    }

    IEnumerator ShrinkTowardsGround()
    {
        Vector3 initialScale = transform.localScale;

        //desactivar la gravedad mientras se encoge para que no flote hacia arriba
        rb.useGravity = false;

        //reduce la escala en Y hasta llegar a 0 (o a un valor peque�o para simular el charco de barro)
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
        //espera el tiempo de duraci�n de la animaci�n de muerte
        yield return new WaitForSeconds(dyingDuration); // Ajusta este tiempo seg�n la duraci�n de la animaci�n de muerte

        //despu�s de la animaci�n, destruir al enemigo
        Destroy(gameObject);
    }

    private void DropItems()
    {
        enemiesDefeated++; //aumenta el contador de enemigos derrotados
        bool hasKey = System.Array.Exists(enemiesWithKey, element => element == enemiesDefeated);

        if (hasKey)
        {
            //si el enemigo es el �ltimo, el que tiene la llave de la sala, deja caer la llave y 2 objetos aleatorios
            SpawnObject(keyPrefab, transform.position + Vector3.up * 0.5f);
            SpawnObject(GetRandomItem(), transform.position + Vector3.right * 0.5f);
            SpawnObject(GetRandomItem(), transform.position + Vector3.left * 0.5f);
        }
        else
        {
            //si el enemigo no es el �ltimo de la sala, deja caer 3 objetos aleatorios
            SpawnObject(GetRandomItem(), transform.position + Vector3.forward * 0.5f);
            SpawnObject(GetRandomItem(), transform.position + Vector3.back * 0.5f);
            SpawnObject(GetRandomItem(), transform.position + Vector3.right * 0.5f);
        }
    }

    //m�todo con iuna ternaria para, con un valor aleatorio, si es entre 0.5 y 1 instancie un prefab de la moneda, si es entre 0.5 y 0, una botella de agua
    private GameObject GetRandomItem()
    {
        return Random.value > 0.5f ? coinPrefab : littleWaterBottlePrefab;
    }

    //m�todo para dejar caer los objetos cuando el enemigo muere, recibiendo el objeto que va a dejar caer y la direcci�n hacia donde lo va a dejar caer
    private void SpawnObject(GameObject prefab, Vector3 position)
    {
        GameObject obj = Instantiate(prefab, position, Quaternion.identity);
        AddJumpEffect(obj);
    }

    private void AddJumpEffect(GameObject obj)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();

        //aplicamos una fuerza en una direcci�n aleatoria para que rebote con m�s gracia
        Vector3 jumpDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(3f, 6f), Random.Range(-1f, 1f));
        rb.AddForce(jumpDirection, ForceMode.Impulse);
    }
}
