using UnityEngine;

public class Flipper : MonoBehaviour
{
    [Space][Header("Hinge Settings")]
    [SerializeField] private HingeJoint hinge;
    [SerializeField] private float restPos = 0f;
    [SerializeField] private float pressedPos = 45f;
    [SerializeField] private float hitStrength = 10000f;
    [SerializeField] private float flipperDamper = 150f;

    private JointSpring spring;

    public void OnHold()
    {
        spring.spring = hitStrength;
        spring.damper = flipperDamper;
        spring.targetPosition = pressedPos;

        hinge.spring = spring;
    }

    public void OnRelease()
    {
        spring.targetPosition = restPos;

        hinge.spring = spring;
    }
}