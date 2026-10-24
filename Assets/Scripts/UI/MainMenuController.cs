
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Settings")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Toggle fullscreenToggle;

    [Header("Music")]
    [SerializeField] private AudioSource menuMusic;

    private bool isLoadingGame = false;


    private void Start()
    {
        // Загружаем сохранённые настройки
        float savedVolume =
            PlayerPrefs.GetFloat("MasterVolume", 1f);

        bool savedFullscreen =
            PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        AudioListener.volume = savedVolume;
        Screen.fullScreen = savedFullscreen;

        // Настраиваем ползунок громкости
        if (volumeSlider != null)
        {
            volumeSlider.SetValueWithoutNotify(savedVolume);
            volumeSlider.onValueChanged.AddListener(
                ChangeVolume
            );
        }

        // Настраиваем переключатель экрана
        if (fullscreenToggle != null)
        {
            fullscreenToggle.SetIsOnWithoutNotify(
                savedFullscreen
            );

            fullscreenToggle.onValueChanged.AddListener(
                ChangeFullscreen
            );
        }

        BackToMenu();
    }

    
    public void StartGame()
    {
        if (isLoadingGame)
            return;

        isLoadingGame = true;
        StartCoroutine(FadeOutAndLoadGame());
    }

    private IEnumerator FadeOutAndLoadGame()
    {
        float fadeDuration = 1.2f;

        if (menuMusic != null && menuMusic.isPlaying)
        {
            float startVolume = menuMusic.volume;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;

                menuMusic.volume = Mathf.Lerp(
                    startVolume,
                    0f,
                    Mathf.Clamp01(elapsed / fadeDuration)
                );

                yield return null;
            }

            menuMusic.Stop();
        }

        SceneManager.LoadScene("MainScene");
    }


    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void BackToMenu()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void ChangeVolume(float value)
    {
        AudioListener.volume = value;

        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayerPrefs.Save();
    }

    public void ChangeFullscreen(bool enabled)
    {
        Screen.fullScreen = enabled;

        PlayerPrefs.SetInt(
            "Fullscreen",
            enabled ? 1 : 0
        );

        PlayerPrefs.Save();
    }

    public void ExitGame()
    {
        Debug.Log("Выход из игры");
        Application.Quit();
    }
}

