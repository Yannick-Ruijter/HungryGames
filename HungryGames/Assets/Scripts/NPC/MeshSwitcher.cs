using UnityEngine;

public class MeshSwitcher : MonoBehaviour
{
    [SerializeField] MeshRenderer[] _meshes;
    EntityMeshType _myType;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Entity>().onEntityReady.AddListener(SwitchMesh);
    }

    public void SwitchMesh(Entity type)
    {
        _myType = type.entityMeshType;
        foreach(var mesh in _meshes)
        {
            if(mesh)
            mesh.enabled = false;
        }

        _meshes[(int)_myType - 1].enabled = true;

    }
}
