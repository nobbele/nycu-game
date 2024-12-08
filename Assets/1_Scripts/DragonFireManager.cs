using UnityEngine;

public class DragonFireManager : MonoBehaviour
{
    private Transform mouthTransform;
    private GameObject fireSpawnPoint;
    private ParticleSystem fireEffect;
    public GameObject fireEffectPrefab;

    void Start()
    {
        mouthTransform = transform.Find("Root/Spine01/Spine02/Chest/Neck01/Neck02/Neck03/Head/Jaw01");
        
        if (mouthTransform != null)
        {
            CreateFirePointTransform();
            CreateFireEffect();
        }
        else
        {
            Debug.LogError("Could not find dragon mouth transform!");
        }
    }

    void CreateFirePointTransform()
    {
        fireSpawnPoint = new GameObject("FireSpawnPoint");
        fireSpawnPoint.transform.SetParent(mouthTransform);

        if (fireSpawnPoint != null)
        {
            fireSpawnPoint.transform.localPosition = new Vector3(30, 30, 0);
            fireSpawnPoint.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 130));
            fireSpawnPoint.transform.localScale = new Vector3(100f, 100f, 100f);
        }
    }

    void CreateFireEffect()
    {
        if (fireEffectPrefab != null)
        {
            GameObject fireInstance = Instantiate(fireEffectPrefab, fireSpawnPoint.transform);
            fireEffect = fireInstance.GetComponent<ParticleSystem>();
            fireEffect.Stop();
        }
    }

    public void StartFire()
    {
        if (fireEffect != null)
            fireEffect.Play();
    }

    public void StopFire()
    {
        if (fireEffect != null)
            fireEffect.Stop();
    }
}
