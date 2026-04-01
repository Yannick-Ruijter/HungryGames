using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public string targetSceneName;
    
    public UnityEvent onTransitionBegin = new UnityEvent();
    public UnityEvent onTransitionEnd = new UnityEvent();
    public UnityEvent<float> TransitionTimer = new UnityEvent<float>();
    public UnityEvent<float> ReceiveSceneLoadingProgress = new UnityEvent<float>();
    public UnityEvent onSceneLoadComplete = new UnityEvent();

    public void LoadScene()
    {
        StartCoroutine(BeginLoadScene());
    }
    
    private IEnumerator BeginLoadScene()
    {
        yield return new WaitForEndOfFrame();
        float timer = 0.0f;
        
        onTransitionBegin.Invoke();

        while (timer < 1.0f)
        {
            timer += Time.deltaTime;
            TransitionTimer.Invoke(timer);

            yield return null;
        }

        onTransitionEnd.Invoke();

        AsyncOperation operation = SceneManager.LoadSceneAsync(targetSceneName);

        if (operation == null)
        {
            throw new Exception($"Failed to load scene \"{targetSceneName}\"");
        }

        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            ReceiveSceneLoadingProgress.Invoke(operation.progress);
            yield return null;
        }
        
        onSceneLoadComplete.Invoke();

        operation.allowSceneActivation = true;
    }
}
