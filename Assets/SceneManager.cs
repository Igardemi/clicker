using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class SceneManager : MonoBehaviour
{
    [SerializeField] private GameObject m_login_UI = null;
    [SerializeField] private GameObject m_game_UI = null;
    [SerializeField] private InputField m_userNameInput = null;
    [SerializeField] private Text m_maxScored = null;

    public Text counterText;
    public Button startButton;
    private int clickCount;
    private bool counting;
    private float timer = 10f;
    public string user_Id;

    public void ShowLogin() {
        m_game_UI.SetActive(false);
        m_login_UI.SetActive(true);
    }

    public void SubmitLogin() {
        if (m_userNameInput.text != "") {
            StartCoroutine(getUserId(m_userNameInput.text));
            counterText.text = "Clicks: 0";
            m_login_UI.SetActive(false);
            m_game_UI.SetActive(true);
        }
    }

    IEnumerator getUserId(string username) {
        // URL de la API
        string url = "http://localhost:3000/users/" + username;

        // Crea la solicitud GET a la API
        UnityWebRequest request = UnityWebRequest.Get(url);

        // Envía la solicitud a la API y espera la respuesta
        yield return request.SendWebRequest();

        // Verifica si ocurrió algún error en la solicitud
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            // Obtiene la respuesta de la API
            string userId = request.downloadHandler.text;
            user_Id = userId;
            StartCoroutine(getMaxScore(userId));
        }
    }

    IEnumerator getMaxScore(string id)
    {
        // URL de la API
        string url = "http://localhost:3000/scores/" + id;

        // Crea la solicitud GET a la API
        UnityWebRequest request = UnityWebRequest.Get(url);

        // Envía la solicitud a la API y espera la respuesta
        yield return request.SendWebRequest();

        // Verifica si ocurrió algún error en la solicitud
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            m_maxScored.text = request.downloadHandler.text;
        }
    }

    IEnumerator setNewScore()
    {

            // URL de la API
            string url = "http://localhost:3000/scores/" + user_Id + "/" + counterText.text;
            // Crea la solicitud GET a la API
            UnityWebRequest request = UnityWebRequest.Get(url);

            // Envía la solicitud a la API y espera la respuesta
            yield return request.SendWebRequest();


            // Verifica si ocurrió algún error en la solicitud
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
            }
    }


    void Start()
    {
        startButton.onClick.AddListener(StartCounting);
        ShowLogin();
    }

    void Update()
    {
        if (counting)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                counting = false;
                counterText.text = clickCount.ToString();

                if (int.Parse(counterText.text) > int.Parse(m_maxScored.text)) { 
                    StartCoroutine(setNewScore());
                }

                startButton.interactable = true;
            }
        }
    }

    void StartCounting()
    {
        clickCount = 0;
        counting = true;
        startButton.interactable = false;
        timer = 10f;
    }

    public void OnButtonClick()
    {
        if (counting)
        {
            clickCount++;
        }
    }

}
