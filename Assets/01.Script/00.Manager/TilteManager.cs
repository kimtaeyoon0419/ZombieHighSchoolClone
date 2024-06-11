// # System
using System.Collections;
using System.Collections.Generic;
using TMPro;

// # Unity
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TilteManager : MonoBehaviour
{
    [Header("Login")]
    public GameObject loginPanel;
    public TMP_InputField inputTxt_ID;
    public TMP_InputField inputTxt_passWord;
    private bool loginPanelActive;

    [Header("LoginCheck")]
    public GameObject idCheckPanel;
    public TextMeshProUGUI check_ID;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0) && !loginPanelActive)
        {
            loginPanel.SetActive(true);
            loginPanelActive = true;
        }
    }

    public void InputIDandPassWord()
    {
        GameManager.instance.playerID = inputTxt_ID.text;
        GameManager.instance.passWord = inputTxt_passWord.text;

        loginPanel.SetActive(false);
        check_ID.text = "정말로 " + inputTxt_ID.text + " 로 하시겠습니까?";
        idCheckPanel.SetActive(true);
    }

    public void NextScene(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }

    public void SelectNo()
    {
        idCheckPanel.SetActive(false);
        loginPanel.SetActive(true);
    }

    public void MoreThink()
    {
        idCheckPanel.SetActive(false);
        loginPanelActive = false;
    }
}
