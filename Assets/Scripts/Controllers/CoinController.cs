using UnityEngine;

public class CoinController : MonoBehaviour
{
    [SerializeField]ParticleSystem coinEffect;
    private void OnTriggerEnter(Collider other)
    {
        coinEffect.Play();
    }
}
