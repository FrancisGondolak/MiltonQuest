using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MiltonLogic : MonoBehaviour
{

    public float moveSpeed = 5f;
    public GameObject waterBallPrefab; //prefab del disparo de agua de Milton
    public Transform firePoint; //punto desde donde se origina el disparo
    public float waterBallSpeed = 10f;
    private bool facingLeft = false; //booleano para saber en qué dirección mira Milton. De inicio en false porque mira a la derecha
    public WaterCounterUI waterCounter; //accedemos a la clase WaterCounterUI para que el contador de agua sea afectado cuando Milton dispare o recoja/use botellas de agua
    public InventoryManager inventoryManager;//accedemos a la clase InventoryManager para afectar a las monedas cuando recojamos monedas en el juego
    public MenuManager menuManager;//acceder a la clase MenuManager
    private bool GamePaused;//variable para controlar cuándo está pausado o no el juego
    public float shootAnimationDuration = 0.5f; //duración de la animación de disparo

    private bool isFlipping = false; //booleano para evitar que se interrumpa la animación de girarse hacia el otro lado
    public float flipSpeed = 0.2f; //velocidad del giro

    public int maxHealth = 3; //vida máxima de Milton (3 corazanas)
    public int currentHealth; //vida actual
    public Image[] heartIcons; //array de imágenes de los corazones en la UI
    public Sprite fullHeartSprite; //sprite de corazón lleno
    public Sprite bittenHeartSprite; //sprite de corazón mordisqueado

    public Animator animator; //Animator para cambiar entre las distintas animaciones

    private bool isDead = false; //para evitar que se sigan ejecutando acciones tras la muerte
    public bool isMoving = false;

    private bool isInvulnerable = false; //para evitar recibir daño en bucle
    public float invulnerabilityDuration = 2f; //duración de la invulnerabilidad tras recibir daño
    private Collider miltonCollider; //referencia al Collider de Milton

    private Rigidbody rb;//variable que va a contener el objeto Milton

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        miltonCollider = GetComponent<Collider>(); //obtiene el Collider de Milton
        currentHealth = maxHealth;
        UpdateHeartsUI();
        GamePaused = false;
    }

    void Update()
    {
        //movimiento del personaje
        float moveX = Input.GetAxis("Horizontal");  //movimiento a izquierda (con A/D o flechas izquierda/derecha)
        float moveZ = Input.GetAxis("Vertical");    //movimiento hacia delante o hacia el fondo, en vertical (con W/S o flechas arriba/abajo)

        //si Milton se mueve, cambiamos a la animación de movimiento
        if (moveX != 0 || moveZ != 0)
        {
            animator.SetBool("isWalking", true);
            isMoving = true; //para saber cuando se está moviendo y que la animación de disparo sea la de moverse y disparar
        }
        else
        {
            animator.SetBool("isWalking", false); //si nos quedamos quietos, cambiamos a la animación de idle
            isMoving= false; //para la animación de disparo en idle
        }

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

        if(Input.GetKeyDown(KeyCode.Escape)) 
        {
            if (GamePaused == false)
            {
                menuManager.OpenPauseMenuCanvas();
            }
            else
            {
                menuManager.ClosePauseMenuCanvas();
            }

            GamePaused = !GamePaused; 

        }
    }

    void ShootWaterBall()
    {
        //comprobar si tenemos agua disponible para disparar
        if (waterCounter.GetCurrentWater() > 0)
        {
            waterCounter.UseWater();

            if(isMoving)
            {
                animator.SetBool("walkingAttack", true);
            }else
            {
                animator.SetBool("idleAttack", true);
            }

            StartCoroutine(WaitForShootAnimation());

            //instancia la bola de agua en el punto de disparo
            GameObject waterBall = Instantiate(waterBallPrefab, firePoint.position, Quaternion.identity);

            //añade velocidad al disparo de agua hacia delante
            Rigidbody waterBallRb = waterBall.GetComponent<Rigidbody>();

            //la dirección del disparo depende de la dirección en la que esté mirando el personaje
            float direction = facingLeft ? 1 : -1;

            waterBallRb.linearVelocity = new Vector3(direction * waterBallSpeed, 0, 0);

            //giramos el sprite del disparo de agua si mira a la izquierda
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

    //método para recoger los objetos que dejen caer los enemigos
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
            inventoryManager.coins += 5; //aumenta el contador de monedas
            inventoryManager.UpdateInventoryUI(); //llama al método del inventario para actualizarlo con las monedas sumadas
        }

        if (other.gameObject.CompareTag("LittleWaterBottle"))
        {
            Destroy(other.gameObject);
            waterCounter.currentWater += 10; //aumenta la munición de agua al recoger botellas pequeñas
            waterCounter.UpdateUI(); //llama al método del contador de Agua para actualizar la munición
        }

        if (other.gameObject.CompareTag("Key"))
        {
            Destroy(other.gameObject);
            inventoryManager.hasKey = true;
            inventoryManager.UpdateInventoryUI();
        }

        if (other.gameObject.CompareTag("Door"))
        {
            if (inventoryManager.hasKey)
            {
                Door door = other.gameObject.GetComponent<Door>(); //obtiene el script de la puerta
                if (door != null)
                {
                    transform.position = door.GetDestination().position; //teletransporta a Milton al punto de la siguiente sala
                    inventoryManager.hasKey = false; //pierde la llave al cambiar de sala
                    inventoryManager.UpdateInventoryUI();
                }
            }
            else
            {
                inventoryManager.ShowMessage("No tienes la llave");
            }
        }
    }

    //método para recibir daño
    public void TakeDamage(Vector2 damageSource)
    {
        if (isDead || isInvulnerable)
        {
            return; //evitar daño si ya está muerto o es invulnerable
        }

        currentHealth--;
        UpdateHeartsUI();

        //activa la invulnerabilidad durante tres segundos tras recibir daño
        StartCoroutine(InvulnerabilityFrames());

        //si la vida llega a 0, activar método Die() para el Game Over
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    //método para actualizar la UI de corazanas
    private void UpdateHeartsUI()
    {
        for (int i = 0; i < heartIcons.Length; i++)
        {
            if (i < currentHealth)
            {
                heartIcons[i].sprite = fullHeartSprite; //corazana llena
            }
            else
            {
                heartIcons[i].sprite = bittenHeartSprite; //corazana mordisqueada
            }
        }
    }

    //método para restaurar vida al usar la corazana
    public void Heal()
    {
        if (currentHealth < maxHealth)
        {
            currentHealth++;
            UpdateHeartsUI();
        }
        else
        {
            Debug.Log("Vida de Milton completa");
        }
    }

    //método para la animación de muerte y Game Over
    private void Die()
    {
        isDead = true;
        animator.SetBool("isDeath", true);

        //detener el movimiento de Milton
        rb.linearVelocity = Vector3.zero; //detener el movimiento físico
        rb.isKinematic = true; //poner el Rigidbody como cinemático para evitar colisiones

        //desactivar entradas de movimiento para evitar que Milton se mueva durante la animación
        enabled = false; //desactivar el script completo (deshabilita la actualización del movimiento)

        StartCoroutine(ShowGameOverMenu());
    }

    //corutina para la animación de disparo
    IEnumerator WaitForShootAnimation()
    {
        //esperamos el tiempo de duración de la animación de disparo
        yield return new WaitForSeconds(shootAnimationDuration);

        //desactiva las dos animaciones de disparo, sea cual sea la que estaba siendo usada
        animator.SetBool("walkingAttack", false);
        animator.SetBool("idleAttack", false);
    }

    IEnumerator InvulnerabilityFrames()
    {
        isInvulnerable = true;

        enabled = false; //desactiva el script de Milton cuando le hacen daño para que no pueda moverse durante ese tiempo
        rb.linearVelocity = Vector3.zero; //detener el movimiento físico de Milton

        //animación de daño
        animator.SetBool("isHurt", true);

        //buscar enemigos y desactivar colisiones
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null) //verifica si el enemigo sigue existiendo, evitando errores
            {
                Collider enemyCollider = enemy.GetComponent<Collider>();
                if (enemyCollider != null)
                {
                    Physics.IgnoreCollision(miltonCollider, enemyCollider, true);
                }
            }
        }

        yield return new WaitForSeconds(invulnerabilityDuration); //espera el tiempo de invulnerabilidad

        //restaurar colisiones después de la invulnerabilidad
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null) //verifica si el enemigo sigue existiendo, evitando errores
            {
                Collider enemyCollider = enemy.GetComponent<Collider>();
                if (enemyCollider != null)
                {
                    Physics.IgnoreCollision(miltonCollider, enemyCollider, false);
                }
            }
        }

        animator.SetBool("isHurt", false);
        isInvulnerable = false;
        enabled = true; //vuelve a activar el script de Milton
    }

    //corrutina para mostrar el menú de Game Over después de la animación
    IEnumerator ShowGameOverMenu()
    {
        yield return new WaitForSeconds(1.5f); //espera 1.5 segundos para que termine la animación de muerte
        menuManager.OpenGameOverMenuCanvas();
    }

}
