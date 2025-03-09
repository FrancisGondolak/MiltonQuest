using UnityEngine;
using System.Collections;

public class MiltonMovement : MonoBehaviour
{

    public float moveSpeed = 5f;
    public GameObject waterBallPrefab; //prefab del disparo de agua de Milton
    public Transform firePoint; //punto desde donde se origina el disparo
    public float waterBallSpeed = 10f;
    private bool facingLeft = false; //booleano para saber en qué dirección mira Milton. De inicio en false porque mira a la derecha
    public WaterCounterUI waterCounter; //asignamos el contador de agua para que sea afectado cuando Milton dispare o recoja/use botellas de agua

    private bool isFlipping = false; //booleano para evitar que se interrumpa la animación de girarse hacia el otro lado
    public float flipSpeed = 0.2f; //velocidad del giro

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //movimiento del personaje
        float moveX = Input.GetAxis("Horizontal");  //movimiento a izquierda (con A/D o flechas izquierda/derecha)
        float moveZ = Input.GetAxis("Vertical");    //movimiento hacia delante o hacia el fondo, en vertical (con W/S o flechas arriba/abajo)

        //si el jugador se mueve hacia la izquierda (X negativo), se rota el personaje hacia la izquierda y ejecuta la animación de giro
        if (moveX < 0 && !facingLeft && !isFlipping)
        {
            StartCoroutine(FlipAnimation());
        }
        //si el jugador se mueve hacia la derecha (X positivo), se rota el personaje hacia la derecha
        else if (moveX > 0 && facingLeft && !isFlipping)
        {
            StartCoroutine(FlipAnimation());
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
        //comprobar si tenemos agua disponible para disparar
        if (waterCounter.GetCurrentWater() > 0)
        {
            waterCounter.UseWater();
            //instancia la bola de agua en el punto de disparo
            GameObject waterBall = Instantiate(waterBallPrefab, firePoint.position, Quaternion.identity);

            //añade velocidad al disparo de agua hacia delante
            Rigidbody waterBallRb = waterBall.GetComponent<Rigidbody>();

            //la dirección del disparo depende de la dirección en la que esté mirando el personaje
            float direction = facingLeft ? 1 : -1;

            waterBallRb.linearVelocity = new Vector3(direction * waterBallSpeed, 0, 0);

            //firamos el sprite del disparo de agua si mira a la izquierda
            if (facingLeft)
            {
                waterBall.transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }

    }

    //función para girar al personaje cuando cambia de dirección usando una corrutina para que lo haga frame a frame
    IEnumerator FlipAnimation()
    {

        isFlipping = true; //evita que se interrumpa la animación si el jugador intenta moverse antes de que termine

        float duration = flipSpeed; //define la duración de la animación de giro

        float elapsedTime = 0f; //tiempo transcurrido desde que empezó el giro

        Quaternion startRotation = transform.rotation; //guarda la rotación actual del personaje antes del giro

        //define la rotación final según la dirección a la que se va a girar
        //si facingLeft es true, gira a 0° (mirando a la derecha)
        //si facingLeft es false, gira a 180° (mirando a la izquierda)
        Quaternion endRotation = Quaternion.Euler(0, facingLeft ? 0 : 180, 0);
       
        while (elapsedTime < duration)  //bucle que ejecuta la animación de giro durante la duración establecida
        {
            float t = elapsedTime / duration; //calcula el progreso de la animación (valor entre 0 y 1)
            
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, t); //interpola suavemente entre la rotación inicial y final según el progreso

            elapsedTime += Time.deltaTime; //aumenta el tiempo transcurrido en cada frame

            yield return null; //pausa la ejecución de la corrutina hasta el siguiente frame
        }

        transform.rotation = endRotation; //asegura que la rotación final sea exactamente la esperada (corrige posibles imprecisiones)

        facingLeft = !facingLeft; //cambia el valor de facingLeft para reflejar la nueva dirección del personaje

        firePoint.right = facingLeft ? -transform.right : transform.right; //ajusta la dirección del disparo para que se mantenga coherente con la rotación

        isFlipping = false; //permite que la animación de giro pueda ejecutarse nuevamente en el futuro

    }

}
