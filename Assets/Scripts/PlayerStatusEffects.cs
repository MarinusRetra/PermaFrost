using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusEffects : MonoBehaviour
{
    public static PlayerStatusEffects Instance;

    private PlayerHealth _playerHP;

    [SerializeField] private Slider _insanitySlider;
    [SerializeField] private Slider _freezingSlider;

    void Awake()
    {
        Instance = this;
        _playerHP = FindAnyObjectByType<PlayerHealth>();
        
        //Sanity and Frostbite update every second
        StartCoroutine(HandleInsanity());
        StartCoroutine(HandleFrostbite());
    }

    //Sanity related variables
    [Header("Sanity")]
    public int InsanityDeath = 20;
    private int _currentInsanity = 0;
    [SerializeField] private List<string> _insanityCauses = new();

    //Sanity related functions
    private IEnumerator HandleInsanity()
    {
        _insanitySlider.maxValue = InsanityDeath;
        while (true)
        {
            _insanitySlider.gameObject.SetActive(_currentInsanity == 0 ? false : true);
            if (_insanityCauses.Count > 0) { _currentInsanity += 2; }
            if (_insanityCauses.Count == 0 && _currentInsanity > 0) { _currentInsanity -= 1; }
            _insanitySlider.value = _insanitySlider.maxValue - _currentInsanity;

            if (_currentInsanity >= InsanityDeath) { _playerHP.GameOver(); }
            yield return new WaitForSeconds(0.2f);
        }
    }
    public void ManageInsanityCauses(string name, bool remove)
    {
        if (remove)
        {
            _insanityCauses.Remove(name);
            return;
        }
        if (_insanityCauses.Contains(name)) return;
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
    public int _currentFrostbite = 0;
    public List<string> _frostbiteCauses = new();

    //Frostbite related functions
    private IEnumerator HandleFrostbite()
    {
        _freezingSlider.maxValue = FrostbiteDeath;
        while (true)
        {
            _freezingSlider.gameObject.SetActive(_currentFrostbite == 0 ? false: true);
            if (_frostbiteCauses.Count > 0) { _currentFrostbite += (2 * _frostbiteCauses.Count); }
            if (_frostbiteCauses.Count == 0 && _currentFrostbite > 0) { _currentFrostbite--; }
            _freezingSlider.value = _freezingSlider.maxValue - _currentFrostbite;

            if (_currentFrostbite >= FrostbiteDeath) { _playerHP.GameOver(); }

            yield return new WaitForSeconds(0.2f);
        }
    }
    public void ManageFrostbiteCauses(string name, bool remove)
    {
        if (remove)
        {
            _frostbiteCauses.Remove(name);
            return;
        }
        if (_frostbiteCauses.Contains(name)) return;
        _frostbiteCauses.Add(name);
    }
    public void AddInstantFrostbite(int instantAmount)
    {
        _currentFrostbite += instantAmount;
        if( _currentFrostbite < 0 ) { _currentFrostbite = 0; }
    }
}
