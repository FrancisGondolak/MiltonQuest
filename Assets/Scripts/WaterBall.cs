using UnityEngine;

public class WaterBall : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        //destruye el proyectil de agua al chocar contra cualquier cosa que tenga collision
        Destroy(gameObject);
        
    }
}
