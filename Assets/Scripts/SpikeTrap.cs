using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MiltonLogic milton = other.GetComponent<MiltonLogic>();
            if (milton != null)
            {
                milton.TakeDamage(transform.position); //llama al m�todo para hacer da�o a Milton
            }
        }
    }
}
