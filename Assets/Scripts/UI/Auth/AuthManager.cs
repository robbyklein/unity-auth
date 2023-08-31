using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using System;
using UnityEngine.SceneManagement;

public class AuthManager : MonoBehaviour {
    #region Properties
    [SerializeField] private JWTTokenManager tokenManager;
    [SerializeField] private UIDocument uiDocument;

    // UI Elements
    private VisualElement root;
    private VisualElement loginBox;
    private TextField loginEmail;
    private TextField loginPassword;
    private Button loginSubmit;
    private Label loginError;
    private Button loginLink;

    private VisualElement signupBox;
    private TextField signupEmail;
    private TextField signupPassword;
    private Button signupSubmit;
    private Label signupError;
    private Button signupLink;
    #endregion

    #region Structs
    // Structs
    private struct RequestBody {
        public string email;
        public string password;
    }

    private struct ResponseBody {
        public string token;
    }
    #endregion

    #region Lifecycle
    private void OnEnable() {
        // Elements
        root = uiDocument.rootVisualElement;
        loginBox = root.Q("login-box");
        loginSubmit = root.Q<Button>("login-submit");
        loginEmail = root.Q<TextField>("login-email");
        loginPassword = root.Q<TextField>("login-password");
        loginLink = root.Q<Button>("login-link");
        signupBox = root.Q("signup-box");
        signupSubmit = root.Q<Button>("signup-submit");
        signupEmail = root.Q<TextField>("signup-email");
        signupPassword = root.Q<TextField>("signup-password");
        signupLink = root.Q<Button>("signup-link");

        // Hide passwords
        loginPassword.maskChar = '*';
        loginPassword.isPasswordField = true;
        signupPassword.maskChar = '*';
        signupPassword.isPasswordField = true;

        // Events
        loginSubmit.clicked += OnLoginSubmitClick;
        loginLink.clicked += OnLoginLinkClick;
        signupSubmit.clicked += OnSignupSubmitClick;
        signupLink.clicked += OnSignupLinkClick;
    }

    private void OnDisable() {
        loginSubmit.clicked -= OnLoginSubmitClick;
        loginLink.clicked -= OnLoginLinkClick;
        signupSubmit.clicked -= OnSignupSubmitClick;
        signupLink.clicked -= OnSignupLinkClick;
    }
    #endregion

    #region Helpers
    private RequestBody CreateLoginRequestBody() {
        string email = loginEmail.value;
        string password = loginPassword.value;

        RequestBody body = new RequestBody {
            email = email,
            password = password
        };

        return body;
    }

    private RequestBody CreateSignupRequestBody() {
        string email = signupEmail.value;
        string password = signupPassword.value;

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

    private void ResetLoginForm() {
        // Clear old error
        if (loginError != null) loginBox.Remove(loginError);

        // Clear values
        loginEmail.value = "";
        loginPassword.value = "";
    }

    private void ResetSignupForm() {
        // Clear old error
        if (signupError != null) signupBox.Remove(signupError);

        // Clear values
        signupEmail.value = "";
        signupPassword.value = "";
    }

    #endregion

    #region Handlers
    private void OnSignupLinkClick() {
        loginBox.RemoveFromClassList("auth-box--active");
        signupBox.AddToClassList("auth-box--active");
    }

    private void OnLoginLinkClick() {
        signupBox.RemoveFromClassList("auth-box--active");
        loginBox.AddToClassList("auth-box--active");
    }

    private void OnLoginSubmitClick() {
        HandleLoginSubmit().Forget();
    }

    private async UniTaskVoid HandleLoginSubmit() {
        try {
            // Post data
            string response = await HTTP.PostJson(APIRoutes.LOGIN, CreateLoginRequestBody());

            // Parse response
            ResponseBody tokenData = JsonUtility.FromJson<ResponseBody>(response);

            // Store token
            tokenManager.Token = tokenData.token;

            // Reset form
            ResetLoginForm();

            // Send to scene
            await SceneManager.LoadSceneAsync("AuthenticatedScene");
        }
        catch {
            if (loginError == null) {
                loginError = CreateError("Invalid email or password");
                loginBox.Add(loginError);
            }
        }
    }

    private void OnSignupSubmitClick() {
        HandleSignupSubmit().Forget();
    }

    private async UniTaskVoid HandleSignupSubmit() {
        try {
            // Post data
            string response = await HTTP.PostJson(APIRoutes.SIGNUP, CreateSignupRequestBody());

            // Parse response
            ResponseBody tokenData = JsonUtility.FromJson<ResponseBody>(response);

            // Store token
            tokenManager.Token = tokenData.token;

            // Reset form
            ResetSignupForm();

            // Send to scene
            await SceneManager.LoadSceneAsync("AuthenticatedScene");
        }
        catch (Exception ex) {
            Debug.Log($"Error: {ex.Message}");

            if (signupError == null) {
                signupError = CreateError("Enter a valid email and 8+ character password");
                signupBox.Add(signupError);
            }
        }
    }

    #endregion
}
