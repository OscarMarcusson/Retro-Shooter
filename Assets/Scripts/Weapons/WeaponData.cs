using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Data", menuName = "Data/Weapon", order = 0)]
public class WeaponData : ScriptableObject
{
    [Header("Model")]
    public GameObject m_model;
    public float m_verticalOffset = 0f;
    public float m_depthOffset = 0f;

    [Header("Settings")]
    [Tooltip("Will the weapon shoot as long as the trigger is held down, or do you need to click manually for each shot?")] 
        public bool m_automatic = false;
    [Tooltip("Shots per second")] 
        public float m_fireRate = 10f;

    [Header("Animation")]
    public AnimationCurve m_recoilCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    public float m_recoilAngles = 60f;
    public float m_recoilMovement = 0.03f;




    /// <summary>
    /// Creates a Weapon object with the settings from this data object.
    /// </summary>
    /// <param name="parent">Optional transform to child to weapon to.</param>
    /// <param name="position">The position of the weapon, in local space.</param>
    /// <param name="rotation">The rotation of the weapon, in local space.</param>
    /// <returns></returns>
    public Weapon Create(Transform parent, Vector3 position, Vector3 rotation)
    {
        Weapon weapon = GameObject.Instantiate(m_model).AddComponent<Weapon>();

        weapon.transform.parent = parent;
        weapon.transform.localPosition = new Vector3(0.15f, m_verticalOffset, m_depthOffset) + position;
        weapon.transform.localEulerAngles = rotation;

        weapon.Init(this);

        return weapon;
    }
}
