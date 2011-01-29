using UnityEngine;
using System.Collections;

public class StudioBehavior : MonoBehaviour 
{
    public Transform indieDevFemalePrefab;
    public Transform indieDevMalePrefab;
    public AudioClip Sound_explotion;

    public void Start () 
    {
	}

    public void OnMouseClicked(RaycastHit hitInfo)
    {        
        var studioBounds = GetComponent<MeshFilter>().mesh.bounds;

        for (int i = 0; i < 20; ++i)
        {
            var localSpawnPos = RandomUtil.GetPointInBounds(studioBounds);
            var worldSpawnPos = transform.TransformPoint(localSpawnPos);
            //Transform indieDvInstance = (Transform)Instantiate(indieDevPrefab, worldSpawnPos, Quaternion.identity);
            if (Random.value > 0.35)
            {
                Instantiate(indieDevMalePrefab, worldSpawnPos, Quaternion.identity);
            }
            else
            {
                Instantiate(indieDevFemalePrefab, worldSpawnPos, Quaternion.identity);
            }
        }

        audio.PlayOneShot(Sound_explotion);
        StartCoroutine(DestroyAfterDelay(Sound_explotion.length));
    }

    public void Update() 
    {
	
	}

    public IEnumerator DestroyAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}
