using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    private static PlayerAudioManager _instance;

    [SerializeField] private AudioSource _audioSourcePrefab;
    [SerializeField] private int _preallocateLocalAudioSources = 10;
    [SerializeField] private int _preallocateWorldAudioSources = 10;
    [Tooltip("If true, when all preallocated audio sources are already in use will allocate new temporary sources.")]
    [SerializeField] private bool _dynamicAllocWhenExceed = false;
    
    [SerializeField] private SoundEffectLibrary _soundLibrary;

    private Dictionary<string, SoundEffect> _registeredSoundEffects;
    
    private AudioSource[] _localAudioSources;
    private WorldSound[] _worldAudioSources;

    private List<WorldSound> _dynamicallyAllocatedSounds = new List<WorldSound>();

    private void Start()
    {
        _instance = this;
        // Locate existing audio configurations
        
        LocateExistingSoundEffects();
        
        DontDestroyOnLoad(gameObject);
        
        PreallocateAudioSources();
    }

    private void LocateExistingSoundEffects()
    {
        if (_registeredSoundEffects != null)
            return;
        
        #if UNITY_EDITOR
        _soundLibrary.populate();
        #endif
        
        _registeredSoundEffects = new Dictionary<string, SoundEffect>();

        foreach (SoundEffect soundEffect in _soundLibrary.soundEffects)
        {
            if (soundEffect) // Verification just in case the previous filter didn't work
            {
                if (_registeredSoundEffects.ContainsKey(soundEffect.name))
                {
                    Debug.LogError($"AudioManager -> Sound effect with id \"{soundEffect.name}\" has already been registered. Sound effect names should be unique.");
                    continue;
                }
                _registeredSoundEffects.Add(soundEffect.name, soundEffect);
                
                Debug.Log($"AudioManager -> Registered sound effect \"{soundEffect.name}\"");
            }
        }
    }

    private void PreallocateAudioSources()
    {
        _worldAudioSources = new WorldSound[_preallocateWorldAudioSources];
        _localAudioSources = new AudioSource[_preallocateLocalAudioSources];

        for (int index = 0; index < _worldAudioSources.Length; index++)
        {
            _worldAudioSources[index] = new WorldSound()
            {
                audioSource = Instantiate(_audioSourcePrefab, transform),
                position = Vector3.zero,
                trackedObject = null
            };
            _worldAudioSources[index].audioSource.gameObject.name = $"WorldAudioSource({index})";
        }

        for (int index = 0; index < _localAudioSources.Length; index++)
        {
            _localAudioSources[index] = Instantiate(_audioSourcePrefab, transform);
            _localAudioSources[index].gameObject.name = $"LocalAudioSource({index})";
        }
    }

    private void Update()
    {
        for (int index = 0; index < _worldAudioSources.Length; index++)
        {
            WorldSound sound = _worldAudioSources[index];
            if (sound.audioSource.isPlaying && sound.trackedObject)
                sound.audioSource.transform.position = sound.trackedObject.position;
        }
    }
    
    public static void PlaySoundNonAlloc(string id)
    {
        GetInstance().PlaySoundNonAllocInternal(id);
    }
    
    public static void PlaySoundNonAlloc(SoundEffect soundEffect)
    {
        if (!soundEffect)
            return;
        GetInstance().PlaySoundNonAllocInternal(soundEffect);
    }
    
    public static void PlaySoundNonAlloc(SoundEffect soundEffect, Vector3 position)
    {
        if (!soundEffect)
            return;
        GetInstance().PlaySoundNonAllocInternal(soundEffect, null, position);
    }
    
    public static void PlaySoundNonAlloc(SoundEffect soundEffect, Transform target)
    {
        if (!soundEffect)
            return;
        GetInstance().PlaySoundNonAllocInternal(soundEffect, target, target.position);
    }

    private void PlaySoundNonAllocInternal(string id)
    {
        SoundEffect soundEffect = _registeredSoundEffects[id];

        PlaySoundNonAllocInternal(soundEffect);
    }
    
    private void PlaySoundNonAllocInternal(SoundEffect soundEffect)
    {
        if (!soundEffect)
            return;
        if (soundEffect.is3D)
        {
            Debug.LogError($"AudioManager -> 3D sound effects need to be played with either a position or a transform.");
            return;
        }

        AudioClip clip = soundEffect.GetAClip();
        
        AudioSource source = GetAvailableLocal(clip);

        source.clip = clip;

        source.spatialBlend = 0.0f;
        source.volume = soundEffect.volume;
        source.pitch = soundEffect.pitch;
        source.Play();
    }

    private void PlaySoundNonAllocInternal(SoundEffect soundEffect, Transform target, Vector3 position)
    {
        if (!soundEffect)
            return;
        if (!soundEffect.is3D)
        {
            PlaySoundNonAlloc(soundEffect);
            return;
        }
        
        AudioClip clip = soundEffect.GetAClip();
        
        WorldSound source = GetAvailableWorld(clip, target, position);

        source.audioSource.clip = clip;

        source.audioSource.spatialBlend = 1.0f;
        source.audioSource.volume = soundEffect.volume;
        source.audioSource.pitch = soundEffect.pitch;
        source.audioSource.minDistance = soundEffect.minDistance;
        source.audioSource.maxDistance = soundEffect.maxDistance;
        source.audioSource.transform.position = target ? target.position : position;
        source.audioSource.Play();
    }

    public static SoundEffect Find(string id, bool verbose = false)
    {
        GetInstance().LocateExistingSoundEffects();
        if (GetInstance()._registeredSoundEffects.TryGetValue(id, out SoundEffect soundEffect))
        {
            return soundEffect;
        }

        if (verbose)
            Debug.LogError($"AudioManager -> Failed to locate SoundEffect with id \"{id}\"");
        return null;
    }

    private static PlayerAudioManager GetInstance()
    {
        if (_instance)
            return _instance;
        _instance = FindFirstObjectByType<PlayerAudioManager>();
        if (_instance)
            return _instance;
        throw new System.Exception("Failed to locate PlayerAudioManager.");
    }

    private AudioSource GetAvailableLocal(AudioClip clip)
    {
        for (int index = 0; index < _localAudioSources.Length; index++)
        {
            if (!_localAudioSources[index].isPlaying)
                return _localAudioSources[index];
        }

        if (_dynamicAllocWhenExceed) {
            AudioSource source = Instantiate(_audioSourcePrefab);
            Destroy(source.gameObject, clip.length);
            return source;
        }
        return null;
    }
    
    private WorldSound GetAvailableWorld(AudioClip clip, Transform target, Vector3 position)
    {
        for (int index = 0; index < _localAudioSources.Length; index++)
        {
            if (!_worldAudioSources[index].audioSource.isPlaying)
                return _worldAudioSources[index];
        }

        if (_dynamicAllocWhenExceed)
        {
            AudioSource source = Instantiate(_audioSourcePrefab);
            Destroy(source.gameObject, clip.length);
            WorldSound sound = new WorldSound()
            {
                trackedObject = target,
                position = position,
                audioSource = source
            };
            source.transform.position = position;
            if (target)
            {
                TrackPosition trackPosition = source.gameObject.AddComponent<TrackPosition>();
                trackPosition.Initialize(target);
            }

            return sound;
        }
        return null;
    }

    private class WorldSound
    {
        public Transform trackedObject;
        public Vector3 position;
        public AudioSource audioSource;
    }
}
