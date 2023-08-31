using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Networking;

public class LoginManager : MonoBehaviour {
    [SerializeField] private UIDocument uiDocument;
    private VisualElement rootVisualElement;
    private TextField emailField;
    private TextField passwordField;
    private Button submitButton;

    private void OnEnable() {
        rootVisualElement = uiDocument.rootVisualElement;
        submitButton = rootVisualElement.Q<Button>("submit-button");
        emailField = rootVisualElement.Q<TextField>("email");
        passwordField = rootVisualElement.Q<TextField>("password");

        passwordField.maskChar = '*';
        passwordField.isPasswordField = true;

        submitButton.clicked += HandleSubmit;
    }

    private void OnDisable() {
        submitButton.clicked -= HandleSubmit;
    }

    private struct RequestBody {
        public string email;
        public string password;
    }

    private void HandleSubmit() {
        string email = emailField.value;
        string password = passwordField.value;

        StartCoroutine(PostData(email, password));
    }

    private IEnumerator PostData(string email, string password) {
        // Create body struct
        RequestBody body = new RequestBody {
            email = email,
            password = password
        };

        // Convert the payload to JSON
        string jsonBody = JsonUtility.ToJson(body);

        // Create a UnityWebRequest and set it up
        UnityWebRequest www = new UnityWebRequest("http://localhost:3000/login", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        // Send the request and yield until it's done
        yield return www.SendWebRequest();

        // Check if there was an error
        if (www.result == UnityWebRequest.Result.ConnectionError) {
            Debug.LogError("Error: " + www.error);
        }
        else if (www.responseCode != 200) {
            Debug.LogError("Fuck!");
        }
        else {
            Debug.Log("Received: " + www.downloadHandler.text);
        }
    }

}
