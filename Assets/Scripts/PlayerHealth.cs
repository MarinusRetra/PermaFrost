using Gameplay;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float DamageInvincibility = 0.5f;
    public float HealInvincibility = 2.0f;

    private bool _damageInvincible = false;
    private bool _healInvincible = false;

    private bool _isVunerable = false;

    private Camera _cam;

    private void Start()
    {
        _cam = transform.Find("Main Camera").GetComponent<Camera>();
        StartCoroutine(MakeSurePlayerDoesntFall());
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

        print("Ow owwieee ouch");
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
        print("Player is die");
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

    private IEnumerator MakeSurePlayerDoesntFall()
    {
        while(true)
        {
            yield return new WaitForSeconds(1);
            if (transform.position.y < 0)
            {
                if (_cam.fieldOfView < 60) continue;
                print("Why is bro so low");
                while(_cam.fieldOfView > 5)
                {
                    yield return new WaitForSeconds(0.1f);
                    _cam.fieldOfView -= 1;
                }
                yield return new WaitForSeconds(1);
                //Send to current room
                while (_cam.fieldOfView < 60)
                {
                    yield return new WaitForSeconds(0.1f);
                    _cam.fieldOfView += 1;
                }
            }
        }
    }
}
