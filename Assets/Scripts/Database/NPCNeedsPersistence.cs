
using System;
using System.Text;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class NPCNeedsPersistence : MonoBehaviour
{
    [Header("Character")]
    [SerializeField] private NPCNeeds needs;

    [Header("PostgreSQL API")]
    [SerializeField] private string apiUrl =
        "http://localhost:5076/api/characters/1/needs";

    [Header("Auto Save")]
    [SerializeField] private float saveInterval = 10f;

    // Показывает, что данные успешно загружены.
    private bool dataLoaded = false;

    [Serializable]
    private class NeedEntry
    {
        public int needTypeId;
        public float value;
    }

    [Serializable]
    private class NeedResponse
    {
        public NeedEntry[] items;
    }

    private IEnumerator Start()
    {
        if (needs == null)
        {
            Debug.LogError("NPCNeeds не назначен!");
            yield break;
        }

        // Сначала загружаем значения из PostgreSQL.
        yield return StartCoroutine(LoadNeeds());

        if (!dataLoaded)
        {
            Debug.LogWarning(
                "Автосохранение отключено: " +
                "не удалось загрузить данные из PostgreSQL.");

            yield break;
        }

        // После успешной загрузки сохраняем
        // значения через заданные интервалы.
        while (true)
        {
            yield return new WaitForSeconds(saveInterval);

            yield return StartCoroutine(SaveNeeds());
        }
    }

    private IEnumerator LoadNeeds()
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
                    "Не удалось загрузить потребности: "
                    + request.error);

                yield break;
            }

            // API возвращает JSON-массив.
            // Оборачиваем его в объект для JsonUtility.
            string json = "{\"items\":"
                + request.downloadHandler.text + "}";

            NeedResponse response;

            try
            {
                response =
                    JsonUtility.FromJson<NeedResponse>(json);
            }
            catch (Exception exception)
            {
                Debug.LogWarning(
                    "Ошибка чтения данных: " +
                    exception.Message);

                yield break;
            }

            if (response == null ||
                response.items == null ||
                response.items.Length != 5)
            {
                Debug.LogWarning(
                    "Получены некорректные данные потребностей.");

                yield break;
            }

            // Применяем загруженные значения к NPC.
            foreach (NeedEntry item in response.items)
            {
                NeedType type;

                switch (item.needTypeId)
                {
                    case 1:
                        type = NeedType.Satiety;
                        break;

                    case 2:
                        type = NeedType.Hygiene;
                        break;

                    case 3:
                        type = NeedType.Sleep;
                        break;

                    case 4:
                        type = NeedType.Work;
                        break;

                    case 5:
                        type = NeedType.Leisure;
                        break;

                    default:
                        Debug.LogWarning(
                            "Неизвестный ID потребности: " +
                            item.needTypeId);
                        continue;
                }

                float difference =
                    item.value - needs.GetValue(type);

                needs.ChangeNeed(type, difference);
            }

            dataLoaded = true;

            Debug.Log(
                "Потребности успешно загружены из PostgreSQL!");
        }
    }

    private IEnumerator SaveNeeds()
    {
        if (!dataLoaded || needs == null)
            yield break;

        NeedType[] types =
        {
            NeedType.Satiety,
            NeedType.Hygiene,
            NeedType.Sleep,
            NeedType.Work,
            NeedType.Leisure
        };

        string[] entries = new string[types.Length];

        for (int i = 0; i < types.Length; i++)
        {
            NeedEntry entry = new NeedEntry
            {
                needTypeId = i + 1,
                value = Mathf.Clamp(
                    needs.GetValue(types[i]),
                    0f,
                    100f)
            };

            entries[i] = JsonUtility.ToJson(entry);
        }

        // Формируем JSON для нашего PUT API.
        string json =
            "[" + string.Join(",", entries) + "]";

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
                    "Потребности сохранены в PostgreSQL!");
            }
            else
            {
                Debug.LogWarning(
                    "Ошибка сохранения: " +
                    request.error);
            }
        }
    }

    // Вызывается перед выходом в главное меню.
    public IEnumerator SaveBeforeExit()
    {
        Debug.Log("Сохраняем игру перед выходом...");

        if (!dataLoaded)
        {
            Debug.LogWarning(
                "Сохранение пропущено: " +
                "данные из PostgreSQL не были загружены.");

            yield break;
        }

        yield return StartCoroutine(SaveNeeds());
    }
}
