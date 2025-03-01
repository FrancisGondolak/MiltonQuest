using UnityEngine;

public class Milton_movement : MonoBehaviour
{

    public float moveSpeed = 5f;
    public GameObject waterBallPrefab; //prefab del disparo de agua de Milton
    public Transform firePoint; //punto desde donde se origina el disparo
    public float waterBallSpeed = 10f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //movimiento del personaje
        float moveX = Input.GetAxis("Horizontal");  //movimineto a izquierda (con A/D o flechas izquierda/derecha)
        float moveZ = Input.GetAxis("Vertical");    //movimiento hacia delante o hacia el fondo, en vertical (con W/S o flechas arriba/abajo)

        Vector3 moveDirection = new Vector3(-moveX, 0, -moveZ).normalized * moveSpeed; //ejes X y Z en negativo porque están invertidos en el juego, no sé porqué (habría que mirarlo)
        rb.linearVelocity = new Vector3(moveDirection.x, rb.linearVelocity.y, moveDirection.z); //actualizar la velocidad del personaje con la dirección en la que se mueve

        //disparar agua al pulsar la barra espaciadora
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShootWaterBall();
        }
    }

    void ShootWaterBall()
    {
        //instancia la bola de agua en el punto de disparo
        GameObject waterBall = Instantiate(waterBallPrefab, firePoint.position, firePoint.rotation);

        //añade velocidad al disparo de agua hacia delante
        Rigidbody waterBallRb = waterBall.GetComponent<Rigidbody>();
        waterBallRb.linearVelocity = -firePoint.right * waterBallSpeed;
    }

}
