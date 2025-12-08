using Gameplay;
using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance;

    //Invincibility
    [SerializeField] private float _damageInvincibility = 0.5f;
    public float HealInvincibility = 2.0f;
    private bool _damageInvincible = false;
    private bool _healInvincible = false;

    private bool _isVunerable = false;

    [SerializeField]private GameObject _deathUI;
    [SerializeField] private GameObject _hitUI;
    [SerializeField] private InputReader _reader;

    [SerializeField] private TMP_Text _deathText;

    private void Start()
    {
        Instance = this;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            StartCoroutine(DamagePlayer(collision.gameObject.name));
            collision.gameObject.GetComponent<Monster>().Deaggro();
        }
    }

    public IEnumerator DamagePlayer(string cause)
    {
        if (_damageInvincible || _healInvincible) yield break;

        if (_isVunerable) 
        {
            GameOver(cause);
            yield break;
        }
        
        _isVunerable = true;
        _hitUI.SetActive(true);

        //Make sure the player doesnt get multihit by the enemies
        _damageInvincible = true;
        yield return new WaitForSeconds(_damageInvincibility);
        _damageInvincible = false;
    }

    public void GameOver(string deathCause)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        deathCause = deathCause.Replace("(Clone)", "");
        deathCause = deathCause.Replace("Agent", " Please");
        deathCause = deathCause.Replace("HotDude", "FireGuy");
        deathCause = deathCause.Replace("Bone.003", "Lookpick");

        //ui
        _deathUI.SetActive(true);
        transform.Find("CamBrain").parent = _deathUI.transform;
        Destroy(_deathUI.transform.parent.Find("Pause").gameObject);
        _deathText.text = "To: " + deathCause;

        gameObject.SetActive(false);
    }

    public IEnumerator HealPlayer()
    {
        if (_healInvincible) yield break;

        _isVunerable = false;
        _hitUI.SetActive(false);

        _healInvincible = true;
        yield return new WaitForSeconds(HealInvincibility);
        _healInvincible = false;
    }
}
