using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class DayNightCycle : MonoBehaviour
{
    public static DayNightCycle Instance { get; private set; }

    [Header("Referencias")]
    [SerializeField] private Light dirLight;

    [Header("Rotación del sol")]
    [SerializeField] private Vector3 sunRotationOffset = new Vector3(-90f, 0f, 0f);

    [Header("Intensidad")]
    [SerializeField] private AnimationCurve intensityCurve;

    [Header("Color")]
    [SerializeField] private Gradient colorGradient;

    private const float fullDayDurationSecs = 240f; // VELOCIDAD DE SIMUALCION DEL CICLO
    private float startHour = 21;
    private float nightStartsAt = 22f;
    private float dayStartsAt = 6f;

    float hoursPerSecond = 24f / fullDayDurationSecs;

    [SerializeField] public float CurrentHour;
    public bool IsNight {  get; private set; }

    public event Action OnNightStarted;
    public event Action OnDayStarted;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    private void Start()
    {
        if (dirLight == null) Debug.LogError("Asigna la luz en el inspector");
        CurrentHour = startHour;
        IsNight = CheckIsNight(CurrentHour);
    }
    private void Update()
    {
        CurrentHour += Time.deltaTime * hoursPerSecond;

        if (CurrentHour >= 24f)
            CurrentHour -= 24f;

        bool newNight = CheckIsNight(CurrentHour);

        if (newNight != IsNight)
        {
            IsNight = newNight;

            if (IsNight) OnNightStarted?.Invoke();
            else OnDayStarted?.Invoke();
        }
        UpdateSun();
    }

    private bool CheckIsNight(float hour)
    {
        return hour >= nightStartsAt || hour < dayStartsAt;
    }
    private void UpdateSun()
    {
        float time01 =CurrentHour / 24f;

        // ROTACIÓN (sol gira en el cielo)
        float sunAngle = time01 * 360f * .85f; // .85 es un ajust epara que tarde más en anochecer
        dirLight.transform.rotation =
            Quaternion.Euler(sunAngle + sunRotationOffset.x, sunRotationOffset.y, sunRotationOffset.z);

        // INTENSIDAD
        float intensity = intensityCurve.Evaluate(time01);
        dirLight.intensity = intensity;

        // COLOR
        Color color = colorGradient.Evaluate(time01);
        dirLight.color = color;
    }

}
