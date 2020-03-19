using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    WeaponData m_weaponData;
    Vector3 m_startPosition;
    Quaternion m_startRotation;

    Vector3 m_velocity;
    Vector3 m_velocityFast;
    Vector2 m_rotation;
    Vector2 m_rotationDelta;
    List<Recoil> m_recoilList;

    float m_fireTimer;
    float m_noiseTimer;
    float m_idleTimer;

    float m_runInterpolation;
    float m_runTimer;


    /// <summary>
    /// Manually initializes the weapon. This replaces the normal "Start()" function.
    /// </summary>
    /// <param name="weaponData"></param>
    public void Init(WeaponData weaponData)
    {
        m_weaponData = weaponData;
        m_startPosition = transform.localPosition;
        m_startRotation = transform.localRotation;
        m_recoilList = new List<Recoil>();
    }


    void Update()
    {
        m_fireTimer -= Time.deltaTime;
    }

    /// <summary>
    /// Try to fire the weapon. This can be called every frame, but might not fire on the given frame, depending on the settings. 
    /// </summary>
    public void Fire()
    {
        if (m_fireTimer > 0f)
            return;

        m_fireTimer = 1f / m_weaponData.m_fireRate;

        // We currently dont handle automatic / semi-automatic firing. There should be a check for that.

        // Spawn bullet & particles
        // And play some sound :)

        m_recoilList.Add(new Recoil(m_weaponData));
    }

    /// <summary>
    /// Animate the weapon. This should be called during the normal update loop, and only once per frame. This is only meant for viewport updating, and is not optimized for multiple weapons at once. 
    /// </summary>
    /// <param name="unitMovement"></param>
    /// <param name="movementSpeed"></param>
    /// <param name="unitRotation"></param>
    public void Animate(Vector3 unitMovement, float movementSpeed, Vector2 unitRotation, bool grounded)
    {
        transform.localPosition = m_startPosition;
        transform.localRotation = m_startRotation;

        // Movement animation
        m_velocity = Vector3.Lerp(m_velocity, unitMovement, Time.deltaTime * 3f);
        m_velocityFast = Vector3.Lerp(m_velocityFast, unitMovement, Time.deltaTime * 6f);

        float sideways = m_velocityFast.x / movementSpeed;
        float forwards = m_velocityFast.z / movementSpeed;
        float vertical = m_velocityFast.y / Mathf.Abs(Physics.gravity.y);

        transform.Translate(
            sideways * 0.03f,
            vertical * 0.07f, 
            Mathf.Min(forwards, 0f) * 0.03f, 
            Space.Self);

        sideways = m_velocity.x / movementSpeed;
        forwards = m_velocity.z / movementSpeed;
        vertical = m_velocity.y / Mathf.Abs(Physics.gravity.y);

        transform.Rotate(
            (forwards) + (vertical * 2f),
            sideways * -0.5f,
            sideways * -7f, 
            Space.Self);

        // Rotation animation
        m_rotation = Vector2.SmoothDamp(m_rotation, Clamp(unitRotation * 0.1f, 1f), ref m_rotationDelta, 0.4f);
        transform.Rotate(
            m_rotation.x * 2f,
            m_rotation.y * 2f,
            m_rotation.y * -15f,
            Space.Self);

        // Recoil
        float totalRecoil = 0f;
        for(int i = 0; i < m_recoilList.Count; i++)
        {
            totalRecoil += m_recoilList[i].getRecoil;
            if (m_recoilList[i].finished)
            {
                m_recoilList.RemoveAt(i);
                i--;
            }
        }

        transform.Rotate(
            -totalRecoil * m_weaponData.m_recoilAngles,
            0f,
            0f,
            Space.Self);

        transform.Translate(
            0f,
            totalRecoil * m_weaponData.m_recoilMovement,
            -totalRecoil * m_weaponData.m_recoilMovement, 
            Space.Self);

        // Add a slight random position and rotation to the weapon to offset the obviously repeated motions
        m_noiseTimer += Time.deltaTime * 0.3f;
        transform.position += transform.parent.right * (0.5f - Mathf.PerlinNoise(1337f, m_noiseTimer)) * 0.02f;
        transform.position += transform.parent.up * (0.5f - Mathf.PerlinNoise(m_noiseTimer, 0f)) * 0.025f;

        transform.Rotate(
            (0.5f - Mathf.PerlinNoise(86f, m_noiseTimer)) * 2f,
            (0.5f - Mathf.PerlinNoise(-9244f, m_noiseTimer)) * 2f,
            (0.5f - Mathf.PerlinNoise(800f, m_noiseTimer)) * 2f,
            Space.Self);

        // Idle
        m_idleTimer = (m_idleTimer + Time.deltaTime * 2f) % (Mathf.PI*2f);
        AnimateSineCurve(
            m_idleTimer,
            0.004f,
            0.007f,
            0.1f);

        // Running
        float movement = Mathf.Sqrt(sideways * sideways + forwards * forwards);

        if (grounded)
            m_runInterpolation = Mathf.Lerp(
                m_runInterpolation,
                Mathf.Clamp01(movement),
                Time.deltaTime * 15f);
        else
            m_runInterpolation = Mathf.Lerp(
                m_runInterpolation,
                0f,
                Time.deltaTime * 5f);
        
        

        m_runTimer = (m_runTimer + Time.deltaTime * movement * 6f) % (Mathf.PI * 2f);
        AnimateSineCurve(
            m_runTimer, 
            0.01f * m_runInterpolation, 
            0.02f * m_runInterpolation,
            3f * m_runInterpolation);
    }


    void AnimateSineCurve(float timer, float x, float y, float rotation)
    {
        transform.position += transform.parent.up * Mathf.Sin(timer * 2f) * x;
        transform.position += transform.parent.right * Mathf.Sin(timer) * y;
        transform.Rotate(
            Mathf.Cos(timer * 2f - 0.2f) * rotation * 0.5f, 
            Mathf.Sin(timer-0.2f) * rotation, 
            0f, 
            Space.Self);
    }


    Vector2 Clamp(Vector2 v, float max)
    {
        v.x = Clamp(v.x, max);
        v.y = Clamp(v.y, max);
        return v;
    }

    float Clamp(float f, float max)
    {
        if (f == 0) 
            return f;

        if (f > 0) 
            return Mathf.Min(f, max);

        return Mathf.Max(f, -max);
    }
}
