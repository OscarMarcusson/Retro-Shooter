using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    // Public variables
    [Header("Size")]
    [Range(0f, 2f)] public float m_radius = 0.3f;
    [Range(0f, 5f)] public float m_height = 1.8f;

    [Header("Movement")]
    [Range(0f, 30f)] public float m_speed = 15f;
    [Range(0f, 50f)] public float m_groundSpeedInputInterpolationSpeed = 30f;
    [Range(0f, 50f)] public float m_airSpeedInputInterpolationSpeed = 5f;
    [Space()]
    [Range(0f, 10f)] public float m_jumpSpeed = 5f;


    // Accessors
    public Vector3 velocity { get { return m_velocity; } }
    public Vector3 relativeVelocity { get { return transform.InverseTransformVector(m_velocity); } }
    public Vector2 rotation { get { return m_rotation; } }
    public bool grounded { get { return m_characterController.isGrounded; } }

    // Member variables
    Vector3 m_velocity;
    CharacterController m_characterController;
    Vector2 m_rotation;
    float m_jumpTimer;




    void Start()
    {
        // Make double sure we have the correct scale
        transform.localScale = Vector3.one;

        // Use the current rotation of the transform as the start rotation
        m_rotation.x = 0f;
        m_rotation.y = transform.eulerAngles.y;

        // Add a character controller.
        m_characterController = gameObject.AddComponent<CharacterController>();
        m_characterController.skinWidth = 0.02f;
        m_characterController.radius = m_radius - m_characterController.skinWidth;
        m_characterController.height = m_height - m_characterController.skinWidth*2f;
        m_characterController.center = new Vector3(0f, m_height * 0.5f, 0f);

        // Allow "on start" functionality for any class that inherits from Unit.
        OnUnitStart();
    }


    void Update()
    {
        PreUpdate();

        // ROTATION UPDATE
        // Loop the rotation to avoid float precision issues. Should never happen during normal play, but some players are strange...
        Vector2 rotation = GetRelativeRotationInput();
        m_rotation.x = Mathf.Clamp(m_rotation.x + rotation.x, -90f, 90f);
        m_rotation.y = (m_rotation.y + rotation.y) % 360f;
        transform.eulerAngles = new Vector3(0f, m_rotation.y, 0f);

        // MOVEMENT UPDATE
        // Grounded update
        if (m_characterController.isGrounded)
        {
            // 
            m_jumpTimer -= Time.deltaTime;

            // Use almost immediate input
            SetMovementVector(m_groundSpeedInputInterpolationSpeed);

            if (m_jumpTimer <= 0f && GetJumpInput())
            {
                if (m_velocity.y <= 0f) m_velocity.y = m_jumpSpeed;
                else m_velocity.y += m_jumpSpeed;
                m_jumpTimer = 0.1f;
            }
            else
            {
                // Set grounded downwards velocity to 3 to make sure we dont get a "stair" effect on downwards terrain.
                m_velocity.y = Mathf.Lerp(velocity.y, -3f, Time.deltaTime * 20f);
            }
        }
        // Air update
        else
        {
            m_jumpTimer = 0f;
            m_velocity.y -= 20f * Time.deltaTime;

            // Use a smoothed input to give a nicer air control feel
            SetMovementVector(m_airSpeedInputInterpolationSpeed);
        }

        m_characterController.Move(m_velocity * Time.deltaTime);

        PostUpdate();
    }


    void SetMovementVector(float interpolationSpeed)
    {
        interpolationSpeed *= Time.deltaTime;
        Vector2 input = GetWorldSpaceMovementInput();
        float magnitude = input.magnitude;
        if (magnitude > 1f)
            input /= magnitude;

        input *= m_speed;

        m_velocity.x = Mathf.Lerp(m_velocity.x, input.x, interpolationSpeed);
        m_velocity.z = Mathf.Lerp(m_velocity.z, input.y, interpolationSpeed);
    }




    /// <summary>
    /// Rotates a vector from local space to world space, based on the horizontal rotation of the Unit. 
    /// </summary>
    /// <param name="vectorToRotate"></param>
    /// <returns></returns>
    protected Vector2 RotateVector(Vector2 vectorToRotate)
    {
        Vector3 tmp = Quaternion.Euler(0f, m_rotation.y, 0f) * new Vector3(vectorToRotate.x, 0f, vectorToRotate.y);
        return new Vector2(tmp.x, tmp.z);
    }

    /// <summary>
    /// This function should return the given world space input vector for the unit. If you need to translate from local to world, you can call the RotateVector() function.
    /// </summary>
    /// <returns></returns>
    protected virtual Vector2 GetWorldSpaceMovementInput()
    {
        return Vector2.zero;
    }

    /// <summary>
    /// This function should return the relative input vector for the rotation of the unit. x = up and down, y = right and left. THIS INPUT SHOULD BE PER FRAME BASED, you might have to apply Time.deltaTime yourself!
    /// </summary>
    /// <returns></returns>
    protected virtual Vector2 GetRelativeRotationInput()
    {
        return Vector2.zero;
    }

    /// <summary>
    /// This function should return true if the unit should jump this frame. This will only be called during the grounded update.
    /// </summary>
    /// <returns></returns>
    protected virtual bool GetJumpInput()
    {
        return false;
    }

    /// <summary>
    /// Called at the end of the Units start function. Implement start functionality with this function instead of "void Start()".
    /// </summary>
    protected virtual void OnUnitStart() { }

    /// <summary>
    /// Called before the default Unit update loop. Position, rotation, and velocity has NOT been updated. 
    /// </summary>
    protected virtual void PreUpdate() { }

    /// <summary>
    /// Called after the default Unit update loop. Position, rotation, and velocity has been updated. 
    /// </summary>
    protected virtual void PostUpdate() { }




    // The entire following block is used to display collision and direction in the editor. It is not included in the build. 
#if UNITY_EDITOR
    static readonly Color handlesMainColor = new Color(0.3f, 1f, 0.3f);
    static readonly Color handlesSecondaryColor = new Color(0.3f, 1f, 0.3f, 0.2f);

    private void OnDrawGizmos()
    {
        // Don't run all of this unoptimized display stuff when the game runs, this is editor viewport only. 
        if (UnityEditor.EditorApplication.isPlaying)
            return;

        // Draw an invisible cube to allow the area to be selectable in the viewport.
        Gizmos.color = Color.clear;
        Gizmos.DrawCube(
            transform.position + transform.up * m_height * 0.5f,
            new Vector3(m_radius * 2f, m_height, m_radius * 2f));

        // Draw multiple rings to show the height and radius. 
        bool selected = UnityEditor.Selection.activeObject == gameObject;
        Quaternion rotation = transform.rotation * Quaternion.Euler(90f, 0f, 0f);
        for (int i = 0; i <= 10; i++)
        {
            // Fade the center rings, they are there just to make it easier to gauge radius, don't want them to get in the way.
            UnityEditor.Handles.color = (selected && (i == 0 || i == 10)) ? handlesMainColor : handlesSecondaryColor;

            UnityEditor.Handles.CircleHandleCap(
            0,
            transform.position + transform.up * m_height * (i * 0.1f),
            rotation,
            m_radius,
            EventType.Repaint);
        }

        UnityEditor.Handles.ArrowHandleCap(
            0,
            transform.position,
            transform.rotation,
            Mathf.Max(m_height * 0.5f, m_radius * 2f),
            EventType.Repaint);
    }
#endif
}
