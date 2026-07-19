using UnityEngine;

public class MeshSwitcher : MonoBehaviour
{
    [SerializeField] private MeshRenderer[] _meshes;
    private EntityMeshType _myType;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Entity>().onEntityReady.AddListener(SwitchMesh);
    }

    public void SwitchMesh(Entity type)
    {
        _myType = type.entityMeshType;
        for (int i = 0; i < _meshes.Length; i++)
        {
            if (_meshes[i] != null)
                _meshes[i].enabled = false;
        }

        _meshes[(int)_myType - 1].enabled = true;

    }
}
