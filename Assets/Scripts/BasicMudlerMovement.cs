using UnityEngine;

public class BasicMudlerMovement : MonoBehaviour
{
    public float speed = 3f; //velocidad de movimiento del Mudler
    private int direction = 1; //dirección inicial (1 = adelante, -1 = atrás)
    public int health = 3; //vida del enemigo
    private Rigidbody rb;

    public GameObject coinPrefab; //prefab de la moneda
    public GameObject littleWaterBottlePrefab; //prefab de la botella de agua pequeña
    public GameObject keyPrefab; //prefab de la llave

    public static int enemiesDefeated = 0; //contador de Mudlers derrotados por Milton
    public int[] enemiesWithKey = {1,3,6,7}; //números en los que los enemigos tienen que soltar la llave del nivel (en la primera sala el enemigo que hay, en la segunda los dos, en la 4 los 3 y en la última el Mudler especial)

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

    private void TakeDamage()
    {
        health--;

        if (health <= 0)
        {
            DropItems();
            Destroy(gameObject); //elimina al enemigo si su vida llega a 0
        }
    }

    private void DropItems()
    {
        enemiesDefeated++; //aumenta el contador de enemigos derrotados

        //suelta una moneda
        GameObject coin = Instantiate(coinPrefab, transform.position, Quaternion.identity);
        AddJumpEffect(coin);

        //suelta una botella de munición
        GameObject littleWaterBottle = Instantiate(littleWaterBottlePrefab, transform.position + new Vector3(0.5f, 0, 0), Quaternion.identity);
        AddJumpEffect(littleWaterBottle);

        //si este enemigo derrotado coincide con alguno de los números del array de enemigos con llave, suelta la llave
        if (System.Array.Exists(enemiesWithKey, element => element == enemiesDefeated))
        {
            GameObject key = Instantiate(keyPrefab, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
            AddJumpEffect(key);
        }

    }

    private void AddJumpEffect(GameObject obj)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();

        rb.AddForce(new Vector3(Random.Range(-1f, 1f), 5f, Random.Range(-1f, 1f)), ForceMode.Impulse);
    }
}
