using Gameplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusEffects : MonoBehaviour
{
    private PlayerHealth _playerHP;

    [SerializeField] private Slider _insanitySlider;
    [SerializeField] private Slider _freezingSlider;

    public void Start()
    {
        _playerHP = PlrRefs.inst.PlayerHealth;
        //Sanity and Frostbite update every second
        StartCoroutine(HandleInsanity());
        StartCoroutine(HandleFrostbite());
    }

    //Sanity related variables
    [Header("Sanity")]
    public int InsanityDeath = 20;
    [SerializeField] private int _currentInsanity = 0;
    [SerializeField] private List<string> _insanityCauses = new();
    [SerializeField] private AudioChorusFilter _sanityAudioFilter;

    //Sanity related functions
    private IEnumerator HandleInsanity()
    {
        _insanitySlider.maxValue = InsanityDeath;
        _insanitySlider.gameObject.SetActive(_currentInsanity <= 0 ? false : true);
        if (!_sanityAudioFilter) { _sanityAudioFilter = PlrRefs.inst.Camera.GetComponent<AudioChorusFilter>(); };

        //sanity will always be active on the player
        while (true)
        {
            if (_insanityCauses.Count == 0 && _currentInsanity == 0) { yield return new WaitForSeconds(0.2f); continue; }
            
            //gain or lose insanity depending on if there is a cause
            if (_insanityCauses.Count > 0) { _currentInsanity += 2; }
            if (_insanityCauses.Count == 0 && _currentInsanity > 0) { _currentInsanity -= 1; }

            _insanitySlider.value = _insanitySlider.maxValue - _currentInsanity;
            _insanitySlider.gameObject.SetActive(_currentInsanity <= 0 ? false : true);

            float currentPercent = (float)_insanitySlider.value / (float)_insanitySlider.maxValue;

            if (_sanityAudioFilter)
            {
                _sanityAudioFilter.wetMix1 = 1 - currentPercent;
                _sanityAudioFilter.wetMix2 = 1 - currentPercent;
                _sanityAudioFilter.wetMix3 = 1 - currentPercent;
                _sanityAudioFilter.dryMix = 0.5f + (currentPercent / 2);
            }
            else { Debug.LogWarning("Sanity Audio Filter missing, Please add a Chorus filter to the player cambrain!"); }

            if (_currentInsanity >= InsanityDeath) { _playerHP.GameOver($"Sanity: {_insanityCauses[0]}"); }
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
    private IEnumerator HandleFrostbite()
    {
        yield return new WaitForSeconds(1);
        _freezingSlider.maxValue = FrostbiteDeath;
        _freezingSlider.gameObject.SetActive(_currentFrostbite <= 0 ? false : true);
        while (true)
        {
            if (_frostbiteCauses.Count == 0 && _currentFrostbite == 0) { yield return new WaitForSeconds(0.2f); continue; }

            //play sound
            if(_frostbiteCauses.Count == 0) { _playSoundNextTick = true; }
            if(_playSoundNextTick && _frostbiteCauses.Count > 0) { Soundsystem.PlaySound(_freezeSFX,transform.position,false,true,0.2f).transform.parent = transform.parent; _playSoundNextTick = false; }

            //gain or lose frostbite. Unlike insanity you gain more frostbite the more causes you have.
            if (_frostbiteCauses.Count > 0) { _currentFrostbite += (2 * _frostbiteCauses.Count); }
            if (_frostbiteCauses.Count == 0 && _currentFrostbite > 0) { _currentFrostbite -= 3; }

            _freezingSlider.value = _freezingSlider.maxValue - _currentFrostbite;
            _freezingSlider.gameObject.SetActive(_currentFrostbite <= 0 ? false : true);

            if (_currentFrostbite >= FrostbiteDeath) { _playerHP.GameOver($"Frostbite: {_frostbiteCauses[0]}"); }

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
