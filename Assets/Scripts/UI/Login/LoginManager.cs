using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;

public class LoginManager : MonoBehaviour {
    // Components
    [SerializeField] private JWTTokenManager tokenManager;
    [SerializeField] private UIDocument uiDocument;

    // UI Elements
    private VisualElement rootVisualElement;
    private VisualElement boxVisualElement;
    private TextField emailField;
    private TextField passwordField;
    private Button submitButton;
    private Label errorElement;

    // Structs
    private struct RequestBody {
        public string email;
        public string password;
    }

    private struct ResponseBody {
        public string token;
    }

    // Lifecycle
    private void OnEnable() {
        rootVisualElement = uiDocument.rootVisualElement;
        boxVisualElement = rootVisualElement.Q("box");
        submitButton = rootVisualElement.Q<Button>("submit-button");
        emailField = rootVisualElement.Q<TextField>("email");
        passwordField = rootVisualElement.Q<TextField>("password");

        passwordField.maskChar = '*';
        passwordField.isPasswordField = true;

        submitButton.clicked += OnSubmitClick;
    }

    private void OnDisable() {
        submitButton.clicked -= OnSubmitClick;
    }

    // Helpers
    private RequestBody CreateRequestBody() {
        string email = emailField.value;
        string password = passwordField.value;

        RequestBody body = new RequestBody {
            email = email,
            password = password
        };

        return body;
    }

    private Label CreateError(string error) {
        Label errorEl = new Label(error);
        errorEl.AddToClassList("auth-box__error");
        return errorEl;
    }

    // Handlers
    private void OnSubmitClick() {
        HandleSubmit().Forget();
    }

    private async UniTaskVoid HandleSubmit() {
        try {
            // Post data
            string response = await HTTP.PostJson(APIRoutes.LOGIN, CreateRequestBody());

            // Clear old error
            if (errorElement != null) boxVisualElement.Remove(errorElement);

            // Parse response
            ResponseBody tokenData = JsonUtility.FromJson<ResponseBody>(response);

            // Store token
            tokenManager.Token = tokenData.token;
        }
        catch {
            if (errorElement == null) {
                errorElement = CreateError("Invalid email or password");
                boxVisualElement.Add(errorElement);
            }
        }
    }
}
