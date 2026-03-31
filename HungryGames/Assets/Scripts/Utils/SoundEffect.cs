using UnityEngine;

[CreateAssetMenu(fileName = "sfx", menuName = "New Sound Effect")]
public class SoundEffect : ScriptableObject
{
    public AudioClip[] clips;

    public float pitch;
    public float volume;

    public bool is3D = false;
    public float minDistance = 0.0f;
    public float maxDistance = 100.0f;
    
    public AnimationCurve fallOff;

    public AudioClip GetAClip()
    {
        if (clips == null || clips.Length == 0)
            return null;
        if (clips.Length == 1)
            return clips[0];
        return clips[Random.Range(0, clips.Length)];
    }
}
