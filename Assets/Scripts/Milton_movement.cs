using UnityEngine;

public class Milton_movement : MonoBehaviour
{

    public float moveSpeed = 5f;
    public GameObject waterBallPrefab; // Prefab de la bola de agua
    public Transform firePoint; // Punto desde donde se dispara
    public float waterBallSpeed = 10f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Movimiento en X y Z (como un 2D en 3D)
        float moveX = Input.GetAxis("Horizontal");  // A/D o flechas izquierda/derecha
        float moveZ = Input.GetAxis("Vertical");    // W/S o flechas arriba/abajo

        Vector3 moveDirection = new Vector3(-moveX, 0, -moveZ).normalized * moveSpeed;
        rb.linearVelocity = new Vector3(moveDirection.x, rb.linearVelocity.y, moveDirection.z);

        // Disparo con Espacio
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShootWaterBall();
        }
    }

    void ShootWaterBall()
    {
        // Instancia la bola de agua en el firePoint
        GameObject waterBall = Instantiate(waterBallPrefab, firePoint.position, firePoint.rotation);

        // Añade velocidad hacia adelante
        Rigidbody waterBallRb = waterBall.GetComponent<Rigidbody>();
        waterBallRb.linearVelocity = firePoint.forward * waterBallSpeed;
    }

}
