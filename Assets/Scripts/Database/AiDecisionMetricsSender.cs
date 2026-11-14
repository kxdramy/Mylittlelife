
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class AiDecisionMetricsSender : MonoBehaviour
{
    [Header("PostgreSQL API")]
    [SerializeField] private string apiUrl =
        "http://localhost:5076/api/characters/1/ai-decisions";

    [Serializable]
    private class DecisionRequest
    {
        public string actionName;
        public string strategyCode;
        public double utilityScore;
        public double decisionTimeMs;
    }

    // Этот метод будет вызывать UtilityAI,
    // когда персонаж выберет действие.
    public void SendDecision(
        string actionName,
        UtilitySelectionStrategy strategy,
        float utilityScore,
        double decisionTimeMs)
    {
        if (string.IsNullOrWhiteSpace(actionName))
            return;

        DecisionRequest data = new DecisionRequest
        {
            actionName = actionName,
            strategyCode = strategy.ToString(),
            utilityScore = Mathf.Clamp(
                utilityScore, 0f, 100f),
            decisionTimeMs = Math.Max(
                0.0, decisionTimeMs)
        };

        // Отправляем данные отдельной корутиной,
        // чтобы не останавливать работу Utility AI.
        StartCoroutine(SendDecisionCoroutine(data));
    }

    private IEnumerator SendDecisionCoroutine(
        DecisionRequest data)
    {
        string json = JsonUtility.ToJson(data);

        using (UnityWebRequest request =
               new UnityWebRequest(
                   apiUrl,
                   UnityWebRequest.kHttpVerbPOST))
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
                    "Решение ИИ сохранено в PostgreSQL: " +
                    data.actionName +
                    " | Strategy: " + data.strategyCode +
                    " | Utility: " +
                    data.utilityScore.ToString("F1") +
                    " | Time: " +
                    data.decisionTimeMs.ToString("F3") +
                    " ms"
                );
            }
            else
            {
                Debug.LogWarning(
                    "Не удалось сохранить решение ИИ: " +
                    request.error +
                    " | HTTP: " + request.responseCode
                );
            }
        }
    }
}
