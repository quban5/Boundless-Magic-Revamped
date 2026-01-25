using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager Instance { get; private set; }

    private PlayerInputActions actions;

    public bool JumpPressed => actions.Player.Jump.IsPressed();
    public bool ShootClicked => actions.Player.Attack.WasPressedThisFrame();
    public bool restartClicked => actions.Player.Next.WasPressedThisFrame();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        actions = new PlayerInputActions();
    }

    private void OnEnable() => actions.Enable();
    private void OnDisable() => actions.Disable();
}
