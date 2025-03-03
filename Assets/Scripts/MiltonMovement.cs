using UnityEngine;

public class MiltonMovement : MonoBehaviour
{

    public float moveSpeed = 5f;
    public GameObject waterBallPrefab; //prefab del disparo de agua de Milton
    public Transform firePoint; //punto desde donde se origina el disparo
    public float waterBallSpeed = 10f;
    private bool facingLeft = false; //booleano para saber en qué dirección mira Milton. De inicio en false porque mira a la derecha

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

        //si el jugador se mueve hacia la izquierda (X negativo), se rota el personaje hacia la izquierda
        if (moveX < 0 && !facingLeft)
        {
            Flip();
        }
        //si el jugador se mueve hacia la derecha (X positivo), se rota el personaje hacia la derecha
        else if (moveX > 0 && facingLeft)
        {
            Flip();
        }

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

    //función para girar al personaje cuando cambia de dirección
    void Flip()
    {
        facingLeft = !facingLeft; //cambia el booleano al valor contrario al que tenía
        Vector3 localScale = transform.localScale; //obtiene el tamaño actual del personaje
        localScale.x *= -1; //invierte al personaje en el eje X
        transform.localScale = localScale; //aplica el nuevo tamaño del personaje (es decir, girado) y lo transforma

        //ternaria para rotar el punto de disparo 180 grados dependiendo de si el booleano que indica la dirección en la que mira el personaje es true o false
        firePoint.localRotation = Quaternion.Euler(0, facingLeft ? 180f : 0f, 0);

    }

}
