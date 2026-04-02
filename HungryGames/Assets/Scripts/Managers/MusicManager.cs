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
    
    // _menuLoop plays in the menu and transitions into gameplay music when gameplay starts
    
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

        DontDestroyOnLoad(gameObject);

        GameState.onGameStart.AddListener(PlayGameplay);
    }
    
    void Update()
    {
        if (_audioSourceLoop.resource == _menuLoop) // This is the reason it's on DontDestroyOnLoad
        {
            _timeSinceLastLoop += Time.deltaTime; // The time since _menuLoop last looped, lets us play _gameplayStart just when it's done
            if (_timeSinceLastLoop >= 16f) // _menuLoop is exactly 16s long
                _timeSinceLastLoop = 0;
        }
    }

    public void PlayMenu() // Triggers when the main menu loads in. Both at the very start and after game restart.
    {
        if (_audioSourceOneshot.resource == _endMusic)
            StartCoroutine(FadeOut(_audioSourceOneshot));
        _audioSourceLoop.resource = _menuLoop;
        _audioSourceLoop.Play();
    }
    
    public void PlayGameplay() // Triggers when gameplay starts
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
    
    public void PlayGameplayFast() // Triggers when there's only a minute left on the counter
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

    public void EndGameplay() // Triggers when someone wins
    {
        StartCoroutine(FadeOut(_audioSourceLoop, 2));
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
