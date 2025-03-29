using UnityEngine;

public class Door : MonoBehaviour
{
    public Transform destination; //punto donde Milton aparecerá al entrar en esta puerta

    public Transform GetDestination()
    {
        return destination;
    }
}
