using UnityEngine;

public class PlayerSoundTriggers : MonoBehaviour
{
    [SerializeField] private Entity _entity;
    private EntityMeshType _entityType;
    
    void Start()
    {
        _entity.onEntityReady.AddListener(SetEntityType);
    }

    void SetEntityType(Entity entity)
    {
        _entityType = _entity.entityMeshType;
    }

    void Footstep()
    {
        if (_entityType == EntityMeshType.Farmer)
            PlayerAudioManager.PlaySoundNonAlloc("SFX_Farmer_Footstep");
        else
        {
            PlayerAudioManager.PlaySoundNonAlloc("SFX_Vegetables_Footsteps_A");
            PlayerAudioManager.PlaySoundNonAlloc("SFX_Vegetables_Footsteps_B");
        }
    }

    void Jump()
    {
        if (_entityType == EntityMeshType.Farmer)
            PlayerAudioManager.PlaySoundNonAlloc("SFX_Farmer_Footstep");
        if (_entityType == EntityMeshType.Carrot)
        {
            PlayerAudioManager.PlaySoundNonAlloc("SFX_Vegetables_Jump_Carrot_A");
        }
        if (_entityType == EntityMeshType.Lettuce)
        {
            PlayerAudioManager.PlaySoundNonAlloc("SFX_Vegetables_Jump_Lettuce_A");
            PlayerAudioManager.PlaySoundNonAlloc("SFX_Vegetables_Jump_Lettuce_B");
        }
        if (_entityType == EntityMeshType.Potato)
        {
            PlayerAudioManager.PlaySoundNonAlloc("SFX_Vegetables_Jump_Potato_A");
        }
        if (_entityType == EntityMeshType.Tomato)
        {
            PlayerAudioManager.PlaySoundNonAlloc("SFX_Vegetables_Jump_Tomato_A");
            PlayerAudioManager.PlaySoundNonAlloc("SFX_Vegetables_Jump_Tomato_B");
        }
    }

    void Cloth()
    {
        if (_entityType == EntityMeshType.Farmer)
            PlayerAudioManager.PlaySoundNonAlloc("SFX_Farmer_ClothMov");
    }
    
    void Chomp()
    {
        if (_entityType == EntityMeshType.Farmer)
            PlayerAudioManager.PlaySoundNonAlloc("SFX_Farmer_Chomp_Voice");
    }
}
