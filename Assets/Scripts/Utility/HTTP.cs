using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

public class HTTP : MonoBehaviour {
    public static async UniTask<string> PostJson<T>(string url, T body, string authToken = null) where T : struct {
        // Serialize data to JSON
        string jsonData = JsonUtility.ToJson(body);

        using (UnityWebRequest www = new UnityWebRequest(url, "POST")) {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            // Add auth token if provided
            if (!string.IsNullOrEmpty(authToken)) {
                www.SetRequestHeader("Authorization", authToken);
            }

            // Send the request
            await www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success) {
                return www.downloadHandler.text;
            }
            else {
                throw new System.Exception(www.error);
            }
        }
    }
}

