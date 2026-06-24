using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager Instance { get; private set; }

    private PlayerInputActions actions;

    private Vector3 firstPosition; // first touch position
    private Vector3 lastPosition; //last touch position
    private float dragDistance; // minimum distance for swipe to be registered

    private bool swipeUp;
    private bool swipeDown;

    //public bool JumpPressed => actions.Player.Jump.IsPressed();
    public bool ShootClicked => actions.Player.Shoot.WasPressedThisFrame();
    public bool restartClicked => actions.Player.Restart.WasPressedThisFrame();
    public bool downClicked => swipeDown || (actions.Player.Move.ReadValue<Vector2>().y < 0 && actions.Player.Move.WasPressedThisFrame());
    public bool upClicked => swipeUp || (actions.Player.Move.ReadValue<Vector2>().y > 0 && actions.Player.Move.WasPressedThisFrame());

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

    void Start()
    {
        dragDistance = Screen.height * 15 / 100; //dragDistance is 15% height of the screen
    } 

    private void Update()
    {
        HandleTouchInput();

        if (actions.Player.Magic.WasPressedThisFrame())
        {
            SpellManager.Instance.CastSpell();
        }
    }

    private void LateUpdate()
    {
        swipeUp = false;
        swipeDown = false;
    }

    private void HandleTouchInput()
    {
        if (Touchscreen.current == null) return;

        var touch = Touchscreen.current.primaryTouch;

        if (touch.press.wasPressedThisFrame) 
        {
            firstPosition = touch.position.ReadValue();
            lastPosition = touch.position.ReadValue();
        }
        else if (touch.press.isPressed) 
        {
            lastPosition = touch.position.ReadValue();
        }
        else if (touch.press.wasReleasedThisFrame) 
        {
            lastPosition = touch.position.ReadValue();

            if (Mathf.Abs(lastPosition.y - firstPosition.y) > dragDistance)
            {
                if (lastPosition.y > firstPosition.y)
                {
                    Debug.Log("Up Swipe");
                    swipeUp = true; 
                }
                else
                {
                    Debug.Log("Down Swipe");
                    swipeDown = true; 
                }
            }
            else
            {
                Debug.Log("Tap");
            }
        }
    }

    private void OnEnable() => actions.Enable();
    private void OnDisable() => actions.Disable();
}
