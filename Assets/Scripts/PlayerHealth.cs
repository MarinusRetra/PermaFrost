using Gameplay;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance;
    public float DamageInvincibility = 0.5f;
    public float HealInvincibility = 2.0f;

    private bool _damageInvincible = false;
    private bool _healInvincible = false;

    private bool _isVunerable = false;

    private Camera _cam;

    private void Start()
    {
        Instance = this;
        _cam = Camera.main;
        StartCoroutine(PlayerOutOfBoundsCheck());
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            print(collision.transform.name + "Has hit the player");
            StartCoroutine(DamagePlayer());
            collision.gameObject.GetComponent<Monster>().Deaggro();
        }
    }

    public IEnumerator DamagePlayer()
    {
        if (_damageInvincible || _healInvincible) yield break;

        print("Player has been hit");
        if (_isVunerable) 
        {
            GameOver();
            yield break;
        }
        
        _isVunerable = true;

        //Make sure the player doesnt get multihit by the enemies
        _damageInvincible = true;
        yield return new WaitForSeconds(DamageInvincibility);
        _damageInvincible = false;
    }

    public void GameOver()
    {
        Debug.LogWarning("Player death is not implemented yet.");
    }

    public IEnumerator HealPlayer()
    {
        if (_healInvincible) yield break;

        //Handle anything with items before this or in here

        _isVunerable = false;

        _healInvincible = true;
        yield return new WaitForSeconds(HealInvincibility);
        _healInvincible = false;
    }

    private IEnumerator PlayerOutOfBoundsCheck()
    {
        while(true)
        {
            yield return new WaitForSeconds(1);
            if (transform.position.y < 0)
            {
                print("Player below the map");
            }
        }
    }
}
