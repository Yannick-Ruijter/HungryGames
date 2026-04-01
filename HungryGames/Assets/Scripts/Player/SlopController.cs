using UnityEngine;

public class SlopController : MonoBehaviour
{
    [SerializeField] private PlayerController m_PlayerController;
    void Start()
    {
        m_PlayerController.GiveMeInputElseWhere += GetControllerInput;
    }

    private ControllerInput GetControllerInput()
    {
        return new ControllerInput()
        {
            move = Vector2.up,
            jump = false
        };
    }
}
