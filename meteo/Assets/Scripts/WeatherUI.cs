using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using TMPro;
using System.Collections;
using UnityEngine.Networking;

public class WeatherUI : MonoBehaviour
{
    [Header("Canvas")]
    public GameObject weatherCanvas;

    [Header("Textes")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI tempText;
    public TextMeshProUGUI windText;
    public TextMeshProUGUI rainText;
    public TextMeshProUGUI statusText;

    [Header("Localisation (optionnel - laisse 0 pour GPS auto)")]
    public float manualLatitude = 0f;
    public float manualLongitude = 0f;

    private XRGrabInteractable grabInteractable;
    private float latitude;
    private float longitude;
    private bool locationReady = false;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);

        // Cache le canvas au départ
        weatherCanvas.SetActive(false);

        // Utilise position manuelle ou GPS
        if (manualLatitude != 0f && manualLongitude != 0f)
        {
            latitude = manualLatitude;
            longitude = manualLongitude;
            locationReady = true;
        }
        else
        {
            StartCoroutine(StartLocationService());
        }
    }

    IEnumerator StartLocationService()
    {
        // Demande la permission GPS
        if (!Input.location.isEnabledByUser)
        {
            // GPS non disponible, utilise une position par défaut (Paris)
            latitude = 48.8566f;
            longitude = 2.3522f;
            locationReady = true;
            Debug.Log("GPS non disponible, utilisation de Paris par défaut.");
            yield break;
        }

        Input.location.Start();

        int maxWait = 10;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (Input.location.status == LocationServiceStatus.Running)
        {
            latitude = Input.location.lastData.latitude;
            longitude = Input.location.lastData.longitude;
            locationReady = true;
            Debug.Log($"GPS OK : {latitude}, {longitude}");
        }
        else
        {
            // Fallback Paris
            latitude = 48.8566f;
            longitude = 2.3522f;
            locationReady = true;
            Debug.Log("GPS échoué, utilisation de Paris par défaut.");
        }
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        weatherCanvas.SetActive(true);

        if (locationReady)
        {
            StartCoroutine(FetchWeather());
        }
        else
        {
            statusText.text = "Localisation en cours...";
            StartCoroutine(WaitForLocationThenFetch());
        }
    }

    void OnRelease(SelectExitEventArgs args)
    {
        weatherCanvas.SetActive(false);
        StopAllCoroutines();
    }

    IEnumerator WaitForLocationThenFetch()
    {
        while (!locationReady)
            yield return new WaitForSeconds(0.5f);

        StartCoroutine(FetchWeather());
    }

    IEnumerator FetchWeather()
    {
        statusText.text = "Chargement...";
        tempText.text = "";
        windText.text = "";
        rainText.text = "";

        string url = $"https://api.open-meteo.com/v1/forecast" +
                     $"?latitude={latitude.ToString("F4", System.Globalization.CultureInfo.InvariantCulture)}" +
                     $"&longitude={longitude.ToString("F4", System.Globalization.CultureInfo.InvariantCulture)}" +
                     $"&current=temperature_2m,windspeed_10m,precipitation" +
                     $"&daily=temperature_2m_max,temperature_2m_min,precipitation_sum,windspeed_10m_max" +
                     $"&timezone=auto&forecast_days=3";

        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                statusText.text = "Erreur réseau";
                Debug.LogError("Erreur météo : " + req.error);
                yield break;
            }

            string json = req.downloadHandler.text;
            WeatherResponse data = JsonUtility.FromJson<WeatherResponse>(json);

            // Affiche les données
            statusText.text = "";
            titleText.text = $"📍 {latitude:F1}°, {longitude:F1}°";

            tempText.text = $"🌡 Maintenant : {data.current.temperature_2m}°C\n" +
                           $"↑ {data.daily.temperature_2m_max[0]}°C  " +
                           $"↓ {data.daily.temperature_2m_min[0]}°C";

            windText.text = $"💨 Vent : {data.current.windspeed_10m} km/h";

            rainText.text = $"🌧 Pluie aujourd'hui : {data.daily.precipitation_sum[0]} mm\n" +
                           $"Demain : {data.daily.precipitation_sum[1]} mm\n" +
                           $"Après-demain : {data.daily.precipitation_sum[2]} mm";
        }
    }

    // Classes pour parser le JSON de Open-Meteo
    [System.Serializable]
    public class WeatherResponse
    {
        public CurrentWeather current;
        public DailyWeather daily;
    }

    [System.Serializable]
    public class CurrentWeather
    {
        public float temperature_2m;
        public float windspeed_10m;
        public float precipitation;
    }

    [System.Serializable]
    public class DailyWeather
    {
        public float[] temperature_2m_max;
        public float[] temperature_2m_min;
        public float[] precipitation_sum;
        public float[] windspeed_10m_max;
    }
}
