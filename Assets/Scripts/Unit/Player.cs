using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
    // Public variables
    [Header("Player Settings")]
    // Temporary public variable to force a given weapon to spawn with the player. This should be replaced by the pickup system when it's built. 
    public WeaponData TEMP_weaponToEquip;

    // Member variables
    Weapon m_weapon;
    Vector2 m_inputRotation;


    protected override void OnUnitStart()
    {
        m_weapon = TEMP_weaponToEquip.Create(Camera.main.transform, Vector3.zero, Vector3.zero);

        // Lock and hide mouse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    protected override void PostUpdate()
    {
        // After movement is done, apply the camera.
        // The camera is shifted backwards slightly, as this gives it a nice "3D" effect when turning
        Camera.main.transform.position = transform.TransformPoint(0f, m_height * 0.8f, 0f);
        Camera.main.transform.Translate(0f, 0f, -m_radius * 0.5f, Space.Self);

        Camera.main.transform.rotation = transform.rotation;
        Camera.main.transform.Rotate(rotation.x, 0f, 0f, Space.Self);

        if (Input.GetKeyDown(KeyCode.Mouse0))
            m_weapon.Fire();

        // Animate weapon
        m_weapon.Animate(
            relativeVelocity, 
            m_speed,
            m_inputRotation / Time.deltaTime, 
            grounded);   // Convert from frame time to second time
    }


    protected override Vector2 GetRelativeRotationInput()
    {
        m_inputRotation = new Vector2(
            -Input.GetAxisRaw("Mouse Y"),
            Input.GetAxisRaw("Mouse X"));

        // Implement proper mouse sense later!
        m_inputRotation *= 3f;

        return m_inputRotation * 3f;
    }


    protected override Vector2 GetWorldSpaceMovementInput()
    {
        return RotateVector(
            new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")));
    }


    protected override bool GetJumpInput()
    {
        return Input.GetButton("Jump");
    }
}
