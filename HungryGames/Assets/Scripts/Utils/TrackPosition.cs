using UnityEngine;

public class TrackPosition : MonoBehaviour
{
    public Transform target { get; private set; }

    public void Initialize(Transform origin)
    {
        if (!target)
            target = origin;
    }
    
    void Update()
    {
        if (target)
            transform.position = target.position;
    }
}
