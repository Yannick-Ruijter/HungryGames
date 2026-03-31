using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "SoundEffectLib", fileName = "SoundEffectLib")]
public class SoundEffectLibrary : ScriptableObject
{
    public List<SoundEffect> soundEffects = new List<SoundEffect>();

    public void populate()
    {
        #if UNITY_EDITOR
        soundEffects.Clear();
        string[] guids = AssetDatabase.FindAssets("t:" + nameof(SoundEffect));

        foreach (string guid in guids)
        {
            SoundEffect soundEffect = (SoundEffect)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(SoundEffect));
            
            soundEffects.Add(soundEffect);
        }
        #endif
    }
}
