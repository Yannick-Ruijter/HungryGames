using System;
using System.Collections.Generic;
using UnityEngine;

public class EntityAnimationController : MonoBehaviour
{
    public enum AnimationType
    {
        none,
        jump,
        walk,
        idle,
        attack
    }

    [System.Serializable]
    public struct AnimationPair
    {
        public AnimationType type;
        public string name;
        public Animation child;
    }

    [SerializeField] private AnimationPair[] animationPairs;

    [SerializeField] private AnimationType type;

    private AnimationType _lastAnimationType = AnimationType.none;
    
    private Dictionary<AnimationType, AnimationPair> animations = new Dictionary<AnimationType, AnimationPair>();

    private void Start()
    {
        foreach (AnimationPair pair in animationPairs)
        {
            animations.Add(pair.type, pair);
            pair.child.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        Play(type);
    }

    private void Play(AnimationType animationName)
    {
        if (animationName == AnimationType.none)
            return;
        if (_lastAnimationType != animationName)
        {
            if (_lastAnimationType != AnimationType.none)
                animations[_lastAnimationType].child.gameObject.SetActive(false);
            animations[animationName].child.gameObject.SetActive(true);
            _lastAnimationType = animationName;
        }
        AnimationPair pair = animations[animationName];

        if (!pair.child.isPlaying)
        {
            pair.child.Play();
        }
    }

    [ContextMenu("Fill in")]
    private void fill()
    {
        Animation[] animations = transform.GetComponentsInChildren<Animation>();
        
        animationPairs = new AnimationPair[animations.Length];

        for (int index = 0; index < animations.Length; index++)
        {
            animationPairs[index] = new AnimationPair()
            {
                child = animations[index],
                type = figureoutbyname(animations[index].gameObject.name),
            };
        }
    }

    private AnimationType figureoutbyname(string name)
    {
        if (name.Contains("walk"))
            return AnimationType.walk;
        if (name.Contains("idle"))
            return AnimationType.idle;
        if (name.Contains("jump"))
            return AnimationType.jump;
        if (name.Contains("attack"))
            return AnimationType.attack;
        return AnimationType.none;
    }
}
