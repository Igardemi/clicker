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
    [SerializeField] public Button startButton = null;
    public Text counterText;
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
            counterText.text = "0";
            m_login_UI.SetActive(false);
            m_game_UI.SetActive(true);
        }
    }
    /**
     * Devuelte la maxiuma puntuación del usuario cuando ingresas el username, si no existe el usuario, lo crea.
     */
    IEnumerator getUserId(string username) {

        string url = "http://localhost:3000/users/" + username;
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();
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

    /**
     * Recupera la mayor puntución de un usuario pasando la id.
     */
    IEnumerator getMaxScore(string id)
    {
        string url = "http://localhost:3000/scores/" + id;
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            m_maxScored.text = request.downloadHandler.text;
        }
    }

    /**
     * Si el score es mayor que el record de este usuario, lo guardará en la base de datos.
     */
    IEnumerator setNewScore()
    {
            string url = "http://localhost:3000/scores/" + user_Id + "/" + counterText.text;
            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();
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

    public void StartCounting()
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
