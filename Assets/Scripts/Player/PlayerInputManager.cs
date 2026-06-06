using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager Instance { get; private set; }

    private PlayerInputActions actions;

    //public bool JumpPressed => actions.Player.Jump.IsPressed();
    public bool ShootClicked => actions.Player.Jump.WasPressedThisFrame();
    public bool restartClicked => actions.Player.Next.WasPressedThisFrame();
    public bool downClicked => actions.Player.Move.ReadValue<Vector2>().y < 0 && actions.Player.Move.WasPressedThisFrame();
    public bool upClicked => actions.Player.Move.ReadValue<Vector2>().y > 0 && actions.Player.Move.WasPressedThisFrame();

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

    private void Update()
    {
        if (actions.Player.Magic.WasPressedThisFrame())
        {
            GameManager.Instance.CastSpell();
        }
    }

    private void OnEnable() => actions.Enable();
    private void OnDisable() => actions.Disable();
}
