using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PerformanceAPI : MonoBehaviour
{
    [System.Serializable]
    public class PerformanceRequest
    {
        public float score;
        public float response_time;
        public int attempts;
    }

    [System.Serializable]
    public class PerformanceResponse
    {
        public string performance;
    }

    public IEnumerator SendData(
        float score,
        float responseTime,
        int attempts,
        System.Action<string> onResult
    )
    {
        PerformanceRequest data = new PerformanceRequest
        {
            score = score,
            response_time = responseTime,
            attempts = attempts
        };

        string json = JsonUtility.ToJson(data);

        UnityWebRequest request = new UnityWebRequest(
            "http://127.0.0.1:5000/predict",
            "POST"
        );

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            PerformanceResponse response =
                JsonUtility.FromJson<PerformanceResponse>(
                    request.downloadHandler.text
                );

            onResult?.Invoke(response.performance);
        }
        else
        {
            onResult?.Invoke("Prediction Failed");
        }
    }
}

