using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;

public class DayNightCycle : MonoBehaviour
{
    public static DayNightCycle Instance { get; private set; }

    [Header("Referencias")]
    [SerializeField] private Light dirLight;
    [SerializeField] private TextMeshProUGUI UI_timerText;
    [SerializeField] private GameObject sun_img;
    [SerializeField] private GameObject moon_img;

    [Header("Rotación del sol")]
    [SerializeField] private Vector3 sunRotationOffset = new Vector3(-90f, 0f, 0f);

    [Header("Intensidad")]
    [SerializeField] private AnimationCurve intensityCurve;

    [Header("Color")]
    [SerializeField] private Gradient colorGradient;
    public float WeatherLightMultiplier { get; set; } = 1f; // Para cuando llueva

    private const float fullDayDurationSecs = 120; // VELOCIDAD DE SIMUALCION DEL CICLO
    [SerializeField] private float startHour = 12;
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
        UpdateSun();
        // Suscribimos funciones
        OnDayStarted += ChangeDayIcon;
        OnNightStarted += ChangeNightIcon;                

        if (dirLight == null) Debug.LogError("Asigna la luz en el inspector");
        if (UI_timerText == null) Debug.LogError("Asigna el TMP de la hora");
        CurrentHour = startHour;
        IsNight = CheckIsNight(CurrentHour);
        sun_img.SetActive(!IsNight);
        moon_img.SetActive(IsNight);
    }
    private void Update()
    {
        CurrentHour += Time.deltaTime * hoursPerSecond;

        if (CurrentHour >= 24f)
            CurrentHour -= 24f;

        // Ajustamos UI
        UI_timerText.text = $"{Mathf.FloorToInt(CurrentHour)} h";

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
        dirLight.intensity = intensity * WeatherLightMultiplier;

        // COLOR
        Color color = colorGradient.Evaluate(time01);
        dirLight.color = color;
    }
    private void ChangeDayIcon()
    {
        UpdateUI(false);
    }
    private void ChangeNightIcon()
    {
        UpdateUI(true);
    }

    void UpdateUI(bool isNight)
    {
        sun_img.SetActive(!isNight);
        moon_img.SetActive(isNight);
    }
}
