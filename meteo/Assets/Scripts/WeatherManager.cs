using UnityEngine;
using UnityEngine.UI;

public class WeatherManager : MonoBehaviour
{
    public string currentWeather;

    private GameObject weatherImageObject;
    private Image weatherImage;
    private AudioSource weatherAudio;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        weatherImageObject = GetComponentInChildren<Image>().gameObject;
        weatherImage = weatherImageObject.GetComponent<Image>();
        weatherAudio = GetComponentInChildren<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentWeather)
        {
            case "pluie":
                if (!weatherImageObject.activeSelf)
                {
                    weatherImageObject.SetActive(true);
                }

                weatherImage.sprite = Resources.Load<Sprite>("Sprites/Weather/rain");
                weatherAudio.clip = Resources.Load<AudioClip>("Sounds/Weather/rain");

                break;
            case "soleil":
                if (!weatherImageObject.activeSelf)
                {
                    weatherImageObject.SetActive(true);
                }

                weatherImage.sprite = Resources.Load<Sprite>("Sprites/Weather/sun");
                weatherAudio.clip = Resources.Load<AudioClip>("Sounds/Weather/sun");

                break;
            default:
                weatherImage.sprite = null;
                weatherAudio.clip = null;

                if (weatherImageObject.activeSelf)
                {
                    weatherImageObject.SetActive(false);
                }

                break;
        }
    }
}
