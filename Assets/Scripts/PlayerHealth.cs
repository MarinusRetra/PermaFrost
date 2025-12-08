using Gameplay;
using System.Collections;
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

    private void Start()
    {
        Instance = this;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            StartCoroutine(DamagePlayer());
            collision.gameObject.GetComponent<Monster>().Deaggro();
        }
    }

    public IEnumerator DamagePlayer()
    {
        if (_damageInvincible || _healInvincible) yield break;

        if (_isVunerable) 
        {
            GameOver();
            yield break;
        }
        
        _isVunerable = true;
        _hitUI.SetActive(true);

        //Make sure the player doesnt get multihit by the enemies
        _damageInvincible = true;
        yield return new WaitForSeconds(_damageInvincibility);
        _damageInvincible = false;
    }

    public void GameOver()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //ui
        _deathUI.SetActive(true);
        transform.Find("CamBrain").parent = _deathUI.transform;
        Destroy(_deathUI.transform.parent.Find("Pause").gameObject);

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
