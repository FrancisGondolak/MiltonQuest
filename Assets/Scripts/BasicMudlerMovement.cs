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
