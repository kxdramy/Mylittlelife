using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenuController : MonoBehaviour
{
    [Header("Database")]
    [SerializeField] private NPCNeedsPersistence needsPersistence;
    
    [Header("Pause")]
    [SerializeField] private GameObject pauseOverlay;
    [SerializeField] private GameObject pauseWindow;
    [SerializeField] private GameObject pauseSettingsWindow;

    [Header("Settings")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Toggle fullscreenToggle;

    private bool isPaused = false;

    private void Awake()
    {
        Time.timeScale = 1f;

        if (pauseOverlay != null)
            pauseOverlay.SetActive(false);

        if (pauseWindow != null)
            pauseWindow.SetActive(true);

        if (pauseSettingsWindow != null)
            pauseSettingsWindow.SetActive(false);
    }

    private void Start()
    {
        float volume =
            PlayerPrefs.GetFloat("MasterVolume", 1f);

        bool fullscreen =
            PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        AudioListener.volume = volume;
        Screen.fullScreen = fullscreen;

        if (volumeSlider != null)
        {
            volumeSlider.SetValueWithoutNotify(volume);
            volumeSlider.onValueChanged.AddListener(ChangeVolume);
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.SetIsOnWithoutNotify(fullscreen);
            fullscreenToggle.onValueChanged.AddListener(
                ChangeFullscreen
            );
        }
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (!Keyboard.current.escapeKey.wasPressedThisFrame)
            return;

        if (!isPaused)
        {
            PauseGame();
        }
        else if (pauseSettingsWindow != null &&
                 pauseSettingsWindow.activeSelf)
        {
            BackToPauseMenu();
        }
        else
        {
            ResumeGame();
        }
    }

    
    public void TogglePause()
    {
        if (!isPaused)
        {
            PauseGame();
        }
        else if (pauseSettingsWindow != null &&
                pauseSettingsWindow.activeSelf)
        {
            BackToPauseMenu();
        }
        else
        {
            ResumeGame();
        }
    }


    public void PauseGame()
    {
        if (pauseOverlay == null)
            return;

        isPaused = true;
        Time.timeScale = 0f;

        if (pauseWindow != null)
            pauseWindow.SetActive(true);

        if (pauseSettingsWindow != null)
            pauseSettingsWindow.SetActive(false);

        pauseOverlay.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pauseOverlay != null)
            pauseOverlay.SetActive(false);
    }

    public void OpenPauseSettings()
    {
        if (pauseWindow != null)
            pauseWindow.SetActive(false);

        if (pauseSettingsWindow != null)
            pauseSettingsWindow.SetActive(true);
    }

    public void BackToPauseMenu()
    {
        if (pauseSettingsWindow != null)
            pauseSettingsWindow.SetActive(false);

        if (pauseWindow != null)
            pauseWindow.SetActive(true);
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

    
    public void GoToMainMenu()
    {
        StartCoroutine(SaveAndReturnToMenu());
    }

    private IEnumerator SaveAndReturnToMenu()
    {
        if (needsPersistence != null)
        {
            yield return StartCoroutine(
                needsPersistence.SaveBeforeExit()
            );
        }
        else
        {
            Debug.LogWarning(
                "NPCNeedsPersistence не подключён!");
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene("StartMenu");
    }


    private void OnDestroy()
    {
        if (volumeSlider != null)
            volumeSlider.onValueChanged.RemoveListener(ChangeVolume);

        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.RemoveListener(
                ChangeFullscreen
            );

        if (isPaused)
            Time.timeScale = 1f;
    }
}
