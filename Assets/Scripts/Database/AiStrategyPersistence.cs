
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.InputSystem;

public class AiStrategyPersistence : MonoBehaviour
{
    [Header("Utility AI")]
    [SerializeField] private UtilityAI utilityAI;

    [Header("PostgreSQL API")]
    [SerializeField] private string apiUrl =
        "http://localhost:5076/api/characters/1/ai-settings";

    private bool strategyLoaded = false;
    private bool isSaving = false;

    [Serializable]
    private class AiSettingsResponse
    {
        public long characterId;
        public string strategyCode;
    }

    [Serializable]
    private class SaveAiStrategyRequest
    {
        public string strategyCode;
    }

    private IEnumerator Start()
    {
        if (utilityAI == null)
        {
            utilityAI = GetComponent<UtilityAI>();
        }

        if (utilityAI == null)
        {
            Debug.LogError(
                "AiStrategyPersistence: UtilityAI не найден!");

            yield break;
        }

        // Загружаем стратегию при запуске игры.
        yield return StartCoroutine(LoadStrategy());
    }

    private IEnumerator LoadStrategy()
    {
        using (UnityWebRequest request =
               UnityWebRequest.Get(apiUrl))
        {
            request.timeout = 10;

            yield return request.SendWebRequest();

            if (request.result !=
                UnityWebRequest.Result.Success)
            {
                Debug.LogWarning(
                    "Не удалось загрузить стратегию ИИ: "
                    + request.error);

                yield break;
            }

            AiSettingsResponse response;

            try
            {
                response =
                    JsonUtility.FromJson<AiSettingsResponse>(
                        request.downloadHandler.text);
            }
            catch (Exception exception)
            {
                Debug.LogWarning(
                    "Ошибка чтения стратегии ИИ: "
                    + exception.Message);

                yield break;
            }

            if (response == null)
            {
                Debug.LogWarning(
                    "Получен пустой ответ от сервера.");

                yield break;
            }

            switch (response.strategyCode)
            {
                case "FirstOnTie":
                    utilityAI.SetStrategy(
                        UtilitySelectionStrategy.FirstOnTie);
                    break;

                case "LastOnTie":
                    utilityAI.SetStrategy(
                        UtilitySelectionStrategy.LastOnTie);
                    break;

                default:
                    Debug.LogWarning(
                        "Неизвестная стратегия: "
                        + response.strategyCode);

                    yield break;
            }

            strategyLoaded = true;

            Debug.Log(
                "Стратегия Utility AI загружена из PostgreSQL: "
                + response.strategyCode);
        }
    }

    private void Update()
    {
        // Пока стратегия не загружена или идёт
        // сохранение, переключение запрещено.
        if (!strategyLoaded || isSaving)
            return;

        if (Keyboard.current == null)
            return;

        // Переключаем стратегию по клавише K.
        if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            StartCoroutine(ChangeAndSaveStrategy());
        }
    }

    private IEnumerator ChangeAndSaveStrategy()
    {
        isSaving = true;

        // Запоминаем предыдущую стратегию.
        UtilitySelectionStrategy previousStrategy =
            utilityAI.CurrentStrategy;

        // Выбираем другую стратегию.
        UtilitySelectionStrategy newStrategy =
            previousStrategy ==
            UtilitySelectionStrategy.FirstOnTie
                ? UtilitySelectionStrategy.LastOnTie
                : UtilitySelectionStrategy.FirstOnTie;

        // Сразу применяем её к персонажу.
        utilityAI.SetStrategy(newStrategy);

        Debug.Log(
            "Переключена стратегия ИИ: " + newStrategy);

        SaveAiStrategyRequest data =
            new SaveAiStrategyRequest
            {
                strategyCode = newStrategy.ToString()
            };

        string json = JsonUtility.ToJson(data);

        // Отправляем PUT-запрос на сервер.
        using (UnityWebRequest request =
               new UnityWebRequest(
                   apiUrl,
                   UnityWebRequest.kHttpVerbPUT))
        {
            request.uploadHandler =
                new UploadHandlerRaw(
                    Encoding.UTF8.GetBytes(json));

            request.downloadHandler =
                new DownloadHandlerBuffer();

            request.SetRequestHeader(
                "Content-Type",
                "application/json");

            request.timeout = 10;

            yield return request.SendWebRequest();

            if (request.result ==
                UnityWebRequest.Result.Success)
            {
                Debug.Log(
                    "Стратегия Utility AI сохранена в PostgreSQL: "
                    + newStrategy);
            }
            else
            {
                // При ошибке возвращаем прежнюю стратегию.
                utilityAI.SetStrategy(previousStrategy);

                Debug.LogWarning(
                    "Ошибка сохранения стратегии ИИ: "
                    + request.error);
            }
        }

        isSaving = false;
    }
}
