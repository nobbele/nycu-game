using UnityEngine;

public class Portal : MonoBehaviour
{
    public Transform destination;

    private ParticleSystem arrivalEffect;
    private bool playerHasExited = true;

    private void Start()
    {
        arrivalEffect = transform.Find("ArrivalEffect").GetComponent<ParticleSystem>();
        arrivalEffect.Pause();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && playerHasExited)
        {
            Portal targetPortal = destination.GetComponent<Portal>();

            if (targetPortal != null)
            {
                targetPortal.PlayArrivalEffect();
                targetPortal.SetPlayerHasExited(false);
            }

            other.transform.position = destination.position;

            playerHasExited = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerHasExited = true;
        }
    }

    public void SetPlayerHasExited(bool state)
    {
        playerHasExited = state;
    }

    public void PlayArrivalEffect()
    {
        if (arrivalEffect != null)
        {
            arrivalEffect.Clear();
            arrivalEffect.Play();
        }
    }
}
