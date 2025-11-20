using UnityEngine;

namespace Gameplay
{
    public class ItemImportance : MonoBehaviour
    {
        [SerializeField] private LayerMask _layer;
        public void OnSpawnKill()
        {
            //This prevents the key spawning on the same spot as other items
            Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale, Quaternion.identity, _layer);

            for(int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].gameObject != gameObject && !hitColliders[i].name.Contains("Key"))
                {
                    Destroy(hitColliders[i].gameObject);
                    Debug.Log("Hit : " + hitColliders[i].name + i);
                }
            }
        }
    }
}
