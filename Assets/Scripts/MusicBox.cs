using System.Collections;
using UnityEngine;

namespace Gameplay
{
    public class MusicBox : MonoBehaviour
    {
        private MeshRenderer outlineVisual;
        [SerializeField] private Vector2 boxRechargeTimes;
        private PlayerStatusEffects _playerEffects;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            outlineVisual = GetComponent<MeshRenderer>();
            SilenceBox();
            _playerEffects = PlayerStatusEffects.Instance;
        }
        private void Update()
        {
            if (_playerEffects.transform.position.z > transform.position.z + 30) { gameObject.SetActive(false); }
        }

        public void SilenceBox()
        {
            if (!outlineVisual.enabled) return;
            print("Silence");
            outlineVisual.enabled = false;
            StartCoroutine(ChargeBox());
        }

        public void StartBox()
        {
            if (!outlineVisual.enabled) return;
            print("Timing minigame");
            _playerEffects.ManageInsanityCauses("Music", false);
        }

        private IEnumerator ChargeBox()
        {
            //let things load
            yield return new WaitForSeconds(0.1f);
            _playerEffects.ManageInsanityCauses("Music", true);
            yield return new WaitForSeconds(Random.Range(boxRechargeTimes.x,boxRechargeTimes.y));
            outlineVisual.enabled = true;
            //Charge sfx
            yield return new WaitForSeconds(1);
            //Actually play
            StartBox();
        }
    }
}
