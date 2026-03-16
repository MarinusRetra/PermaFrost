using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Item/NoiseMonkey")]

    public class NoiseMonkey : InventoryItem
    {
        [SerializeField] private GameObject _noiseMonkeyPrefab;
        public override bool Use()
        {
            GameObject spawnedMonkey = Instantiate(_noiseMonkeyPrefab, PlrRefs.inst.Camera.transform.position, PlrRefs.inst.Camera.transform.rotation);
            Vector3 direction =  spawnedMonkey.transform.forward;
            spawnedMonkey.GetComponent<Rigidbody>().AddForce(direction * 1000);
            return true;
        }
    }
}
