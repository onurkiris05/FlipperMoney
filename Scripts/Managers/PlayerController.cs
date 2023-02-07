using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using VP.Nest.Haptic;
using VP.Nest.SceneManagement;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class PlayerController : MonoBehaviour
{
    [Space][Header("Layer Settings")]
    [SerializeField] private LayerMask tapLayer;

    private Finger movementFinger;
    private Camera cam;
    private RaycastHit hit;
    private Ray ray;

    private bool isHoldingFlippers;

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();

        ETouch.Touch.onFingerDown += HandleFingerDown;
        ETouch.Touch.onFingerUp += HandleFingerUp;
        ETouch.Touch.onFingerMove += HandleFingerMove;

        cam = Camera.main;
    }

    void OnDisable()
    {
        ETouch.Touch.onFingerDown -= HandleFingerDown;
        ETouch.Touch.onFingerUp -= HandleFingerUp;
        ETouch.Touch.onFingerMove -= HandleFingerMove;

        EnhancedTouchSupport.Disable();
    }

    void HandleFingerDown(Finger touchedFinger)
    {
        if (!LevelManager.IsLevelPlaying) return;

        if (movementFinger == null)
        {
            movementFinger = touchedFinger;

            ray = cam.ScreenPointToRay(movementFinger.currentTouch.screenPosition);

            //Trigger flippers on tap screen
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, tapLayer) && !isHoldingFlippers)
            {
                isHoldingFlippers = true;
                FlipperManager.Instance.RaiseFlippers();
                HapticManager.Haptic(HapticType.SoftImpact);
                FTUEManager.Instance.Step3?.Invoke();
            }
        }
    }

    void HandleFingerUp(Finger lostFinger)
    {
        if (lostFinger == movementFinger)
        {
            movementFinger = null;

            //Release flippers
            if (isHoldingFlippers)
            {
                FlipperManager.Instance.ReleaseFlippers();
                isHoldingFlippers = false;
            }
        }
    }

    void HandleFingerMove(Finger movedFinger)
    {
        if (movedFinger == movementFinger)
        {
        }
    }
}