using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "sfx", menuName = "New Sound Effect")]
public class SoundEffect : ScriptableObject
{
    public AudioResource clip;
    public AudioClip sampleClip;

    public float pitch;
    public float volume;

    public bool is3D = false;
    public float minDistance = 0.0f;
    public float maxDistance = 100.0f;
    
    public AnimationCurve fallOff;

    public AudioResource GetAClip()
    {
        return clip;
    }

    public float GetSampleClipLength() // AudioRandomContainer is an AudioResource and doesn't support .length
    {
        return sampleClip.length;
    }
}
