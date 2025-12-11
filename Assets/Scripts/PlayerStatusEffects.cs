using Gameplay;
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
    }

    public void Start()
    {
        //Sanity and Frostbite update every second
        StartCoroutine(HandleInsanity());
        StartCoroutine(HandleFrostbite());
    }

    //Sanity related variables
    [Header("Sanity")]
    public int InsanityDeath = 20;
    [SerializeField] private int _currentInsanity = 0;
    [SerializeField] private List<string> _insanityCauses = new();

    //Sanity related functions
    private IEnumerator HandleInsanity()
    {
        _insanitySlider.maxValue = InsanityDeath;

        //sanity will always be active on the player
        while (true)
        {
            _insanitySlider.gameObject.SetActive(_currentInsanity <= 0 ? false : true);

            //gain or lose insanity depending on if there is a cause
            if (_insanityCauses.Count > 0) { _currentInsanity += 2; }
            if (_insanityCauses.Count == 0 && _currentInsanity > 0) { _currentInsanity -= 1; }

            _insanitySlider.value = _insanitySlider.maxValue - _currentInsanity;

            if (_currentInsanity >= InsanityDeath) { _playerHP.GameOver("Sanity"); }
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

        //This is to prevent players from stacking soothing bells and never having to deal with sanity again
        if(_currentInsanity < -InsanityDeath) { _currentInsanity = -InsanityDeath; }
    }

    //Frostbite related variables
    [Header("Frostbite")]
    public int FrostbiteDeath = 30;
    [SerializeField] private int _currentFrostbite = 0;
    private List<string> _frostbiteCauses = new();

    private bool _playSoundNextTick = true;
    [SerializeField] private AudioClip _freezeSFX;

    //Frostbite related functions
    [SerializeField] private IEnumerator HandleFrostbite()
    {
        yield return new WaitForSeconds(1);
        _freezingSlider.maxValue = FrostbiteDeath;
        while (true)
        {
            _freezingSlider.gameObject.SetActive(_currentFrostbite <= 0 ? false: true);

            //play sound
            if(_frostbiteCauses.Count == 0) { _playSoundNextTick = true; }
            if(_playSoundNextTick && _frostbiteCauses.Count > 0) { Soundsystem.PlaySound(_freezeSFX,transform.position).transform.parent = transform.parent; _playSoundNextTick = false; }

            //gain or lose frostbite. Unlike insanity you gain more frostbite the more causes you have.
            if (_frostbiteCauses.Count > 0) { _currentFrostbite += (2 * _frostbiteCauses.Count); }
            if (_frostbiteCauses.Count == 0 && _currentFrostbite > 0) { _currentFrostbite -= 3; }

            _freezingSlider.value = _freezingSlider.maxValue - _currentFrostbite;

            if (_currentFrostbite >= FrostbiteDeath) { _playerHP.GameOver("Frostbite"); }

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
