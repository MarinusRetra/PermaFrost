using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatusEffects : MonoBehaviour
{
    public static PlayerStatusEffects Instance;

    private PlayerHealth _playerHP;

    void Start()
    {
        _playerHP = FindAnyObjectByType<PlayerHealth>();
        
        //Sanity and Frostbite update every second
        StartCoroutine(HandleInsanity());
        StartCoroutine(HandleFrostbite());
    }

    //Sanity related variables
    [Header("Sanity")]
    public int InsanityDeath = 20;
    private int _currentInsanity = 0;
    private List<string> _insanityCauses = new();

    //Sanity related functions
    private IEnumerator HandleInsanity()
    {
        while (true)
        {
            if (_insanityCauses.Count > 0) { _currentInsanity += 2; }
            if (_insanityCauses.Count == 0 && _currentInsanity > 0) { _currentInsanity--; }

            if (_currentInsanity >= InsanityDeath) { _playerHP.GameOver(); }
            yield return new WaitForSeconds(1);
        }
    }
    public void ManageInsanityCauses(string name, bool remove)
    {
        if (remove)
        {
            _insanityCauses.Remove(name);
            return;
        }
        _insanityCauses.Add(name);
    }
    public void AddInstantInsanity(int instantAmount)
    {
        _currentInsanity += instantAmount;
        if(_currentInsanity < 0) { _currentInsanity = 0; }
    }

    //Frostbite related variables
    [Header("Frostbite")]
    public int FrostbiteDeath = 30;
    private int _currentFrostbite = 0;
    private List<string> _frostbiteCauses = new();

    //Frostbite related functions
    private IEnumerator HandleFrostbite()
    {
        while (true)
        {
            if (_frostbiteCauses.Count > 0) { _currentFrostbite += 2; }
            if (_frostbiteCauses.Count == 0 && _currentFrostbite > 0) { _currentFrostbite--; }

            if (_currentFrostbite >= FrostbiteDeath) { _playerHP.GameOver(); }

            yield return new WaitForSeconds(1);
        }
    }
    public void ManageFrostbiteCauses(string name, bool remove)
    {
        if (remove)
        {
            _frostbiteCauses.Remove(name);
            return;
        }
        _frostbiteCauses.Add(name);
    }
    public void AddInstantFrostbite(int instantAmount)
    {
        _currentFrostbite += instantAmount;
        if( _currentFrostbite < 0 ) { _currentFrostbite = 0; }
    }
}
