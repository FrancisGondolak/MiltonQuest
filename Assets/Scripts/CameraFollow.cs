using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; //referencia al personaje
    public Vector3 offset = new Vector3(0, 3, 7.5f); //ajusta la distancia de la c�mara
    public float smoothSpeed = 10f; //suavidad del movimiento

    void FixedUpdate() //con FixedUpdate la c�mara se mueve en el mismo ciclo que el RigidBody del personaje, evitando que el personaje "tiemble" cuando se ajusta la c�mara al seguirle en cada frame 
    {
        if (player == null) return; //evita errores si no hay personaje

        //calcula la posici�n del objetivo
        Vector3 targetPosition = player.position + offset;

        //movimiento de la c�mara de forma suave
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.fixedDeltaTime);
    }
}
