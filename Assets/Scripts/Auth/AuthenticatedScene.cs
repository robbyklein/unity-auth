using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.SceneManagement;

public class AuthenticatedScene : MonoBehaviour {
    [SerializeField] private JWTTokenManager tokenManager;

    private void Awake() {
        CheckAuthentication().Forget();
    }

    private async UniTaskVoid CheckAuthentication() {
        try {


            // Post data
            string response = await HTTP.PostJson(APIRoutes.VALIDATE, false, tokenManager.Token);
        }
        catch (Exception ex) {
            Debug.Log(ex);
            // await SceneManager.LoadSceneAsync("Login");
        }
    }
}
