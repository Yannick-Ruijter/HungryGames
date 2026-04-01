using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup _mixerGroup;
    
    [SerializeField] private AudioResource _menuLoop;
    [SerializeField] private AudioResource _gameplayStart;
    [SerializeField] private AudioResource _gameplayLoop;
    [SerializeField] private AudioResource _gameplayStartFast;
    [SerializeField] private AudioResource _gameplayLoopFast;
    [SerializeField] private AudioResource _endMusic;
    
    private AudioSource _audioSourceOneshot;
    private AudioSource _audioSourceLoop;

    private float _timeSinceLastLoop = 0;
    
    void Start()
    {
        _audioSourceOneshot = new AudioSource();
        _audioSourceOneshot.outputAudioMixerGroup = _mixerGroup;
        _audioSourceOneshot.playOnAwake = false;
        _audioSourceOneshot.spatialBlend = 0.0f;
        
        _audioSourceLoop = new AudioSource();
        _audioSourceLoop.outputAudioMixerGroup = _mixerGroup;
        _audioSourceLoop.playOnAwake = false;
        _audioSourceLoop.spatialBlend = 0.0f;
    }
    
    void Update()
    {
        if (_audioSourceLoop.resource == _menuLoop)
        {
            _timeSinceLastLoop += Time.deltaTime;
            if (_timeSinceLastLoop >= 16f)
                _timeSinceLastLoop = 0;
        }
    }

    public void PlayMenu()
    {
        _audioSourceLoop.resource = _menuLoop;
        _audioSourceLoop.Play();
    }

    public void PlayGameplay()
    {
        StartCoroutine(PlayGameplayFull());
    }

    IEnumerator PlayGameplayFull()
    {
        yield return new WaitForSeconds(16f - _timeSinceLastLoop);
        _audioSourceOneshot.resource = _gameplayStart;
        _audioSourceOneshot.Play();
        yield return new WaitForSeconds(16); // The menu loop length, don't question it
        _audioSourceLoop.resource = _gameplayLoop;
        _audioSourceLoop.Play();
        yield return null;
    }
    
    public void PlayGameplayFast()
    {
        StartCoroutine(FadeOut(_audioSourceLoop, 1));
    }
    
    IEnumerator PlayGameplayLoopFast()
    {
        yield return new WaitForSeconds(12); // The fast start length, don't question it
        _audioSourceLoop.resource = _gameplayLoopFast;
        _audioSourceLoop.Play();
        yield return null;
    }

    public void EndGameplay()
    {
        StartCoroutine(FadeOut(_audioSourceLoop, 2));
    }

    public void EndGame()
    {
        StartCoroutine(FadeOut(_audioSourceOneshot));
    }
    
    IEnumerator FadeOut(AudioSource audioSource, int gameState = 3, float duration = 1f)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / duration;
            yield return new WaitForSeconds(1f);
        }

        audioSource.Stop();
        audioSource.volume = startVolume;

        if (gameState == 1)
        {
            _audioSourceOneshot.resource = _gameplayStartFast;
            _audioSourceOneshot.Play();
            StartCoroutine(PlayGameplayLoopFast());
        }

        if (gameState == 2)
        {
            _audioSourceOneshot.resource = _endMusic;
            _audioSourceOneshot.Play();
        }
        yield return null;
    }
}
