using UnityEngine;

public class PlayerSoundTriggers : MonoBehaviour
{
    [SerializeField] private AnimationPassthrough _animpass;
    [SerializeField] private Entity _entity;
    private EntityMeshType _entityType;

    private AudioSource _audioSource;

    void Start()
    {
        _animpass.onJump.AddListener(Jump);
        _animpass.OnDefise.AddListener(Defuse);
        _animpass.OnDefiseStop.AddListener(StopDefuse);
        _entity.onEntityReady.AddListener(SetEntityType);

        _audioSource = GetComponent<AudioSource>();
    }

    void SetEntityType(Entity entity)
    {
        _entityType = _entity.entityMeshType;
    }

    public void Footstep()
    {
        if (_entityType == EntityMeshType.Farmer)
            PlayerAudioManager.PlaySoundNonAlloc("SFX_Farmer_Footstep");
        else
        {
            PlayerAudioManager.PlaySoundNonAlloc("SFX_Vegetables_Footsteps_A");
            PlayerAudioManager.PlaySoundNonAlloc("SFX_Vegetables_Footsteps_B");
        }

        Debug.Log("Played footstep");
    }

    public void Jump()
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

        Debug.Log("Played jump");
    }

    public void Cloth()
    {
        if (_entityType == EntityMeshType.Farmer)
            PlayerAudioManager.PlaySoundNonAlloc("SFX_Farmer_ClothMov");
    }

    public void Chomp()
    {
        if (_entityType == EntityMeshType.Farmer)
            PlayerAudioManager.PlaySoundNonAlloc("SFX_Farmer_Chomp_Voice");
    }

    public void Defuse()
    {
        _audioSource.Play();
    }

    public void StopDefuse()
    {
        _audioSource.Stop();
    }
}
