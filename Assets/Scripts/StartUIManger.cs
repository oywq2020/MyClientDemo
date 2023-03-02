using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using  Common;

public class StartUIManger : MonoBehaviour
{
    public static StartUIManger Instance;

    public Transform loginPanel, registerPanel, mainPanel;

    public GameObject hintMessage;

    private InputField _usernameLogin,_ageLogin,_usernameRegister,_ageRegister;
    // Start is called before the first frame update

    private string _currentUsername;
    

    private void Awake()
    {
        Instance = this;
        InitIF();
        InitBtn();
    }

    //Init input field
    private void InitIF()
    {
        _usernameLogin = loginPanel.Find("InputName").GetComponent<InputField>();
        _ageLogin = loginPanel.Find("InputAge").GetComponent<InputField>();
        _usernameRegister = registerPanel.Find("InputName").GetComponent<InputField>();
        _ageRegister = registerPanel.Find("InputAge").GetComponent<InputField>();



    }

    //init click event
    private void InitBtn()
    {

        #region Login

        loginPanel.Find("LoginBtn").GetComponent<Button>().onClick.AddListener(() =>
        {
            //Login 
            //Check Input before send message:NULL check
            if (_usernameLogin.text.Equals("")||_ageLogin.text.Equals(""))
            {
                ShowHint("输入的内容不能为空，请重新输入");
                InitContentLogin();
                return;
            }
            
            Dictionary<byte, object> data = new Dictionary<byte, object>();
            data.Add((byte) ParameterCode.Username,_usernameLogin.text);
            data.Add((byte) ParameterCode.Age,_ageLogin.text);
            
            PhotonManger.Instance.Peer.OpCustom((byte)OperationCode.Login, data, true);
        });
        loginPanel.Find("RegisterBtn").GetComponent<Button>().onClick.AddListener(() =>
        {
            loginPanel.gameObject.SetActive(false);
            registerPanel.gameObject.SetActive(true);
        });

        #endregion

        #region Register

        registerPanel.Find("RegisterBtn").GetComponent<Button>().onClick.AddListener(() =>
        {
            //Register
            //Check Input before send message:NULL check
            if (_usernameRegister.text.Equals("")||_ageRegister.text.Equals(""))
            {
                ShowHint("输入的内容不能为空，请重新输入");
                InitContentRegister();
                return;
            }
            
            Dictionary<byte, object> data = new Dictionary<byte, object>();
            data.Add((byte) ParameterCode.Username,_usernameRegister.text);
            data.Add((byte) ParameterCode.Age,_ageRegister.text);
            
            PhotonManger.Instance.Peer.OpCustom((byte)OperationCode.Register, data, true);
        });
        
        registerPanel.Find("ReturnBtn").GetComponent<Button>().onClick.AddListener(() =>
        {
            loginPanel.gameObject.SetActive(true);
            registerPanel.gameObject.SetActive(false);
        });

        #endregion

        #region Cancel
        
        
        mainPanel.Find("CancelBtn").GetComponent<Button>().onClick.AddListener(() =>
        {
            //Cancel
            //Check Input before send message:NULL check
           
            Dictionary<byte, object> data = new Dictionary<byte, object>();
            data.Add((byte) ParameterCode.Username,_currentUsername);
            
            
            PhotonManger.Instance.Peer.OpCustom((byte)OperationCode.LogOut, data, true);
        });

        mainPanel.Find("ReturnBtn").GetComponent<Button>().onClick.AddListener(() =>
        {
            mainPanel.gameObject.SetActive(false);
            loginPanel.gameObject.SetActive(true);
        });
        #endregion
        
       
    }

    private void ShowHint(string  str)
    {
        hintMessage.SetActive(true);
        hintMessage.GetComponentInChildren<Text>().text = str;
    }

    
    private void InitContentLogin()
    {
        _usernameLogin.text = "";
        _ageLogin.text = "";
    }

    private void InitContentRegister()
    {
        _usernameRegister.text = "";
        _ageRegister.text = "";
    }

    public void OnLogin(ReturnCode returnCode)
    {
        if (returnCode == ReturnCode.Success)
        {
            //verify successfully
            ShowHint("登入成功");
            //update the current username
            _currentUsername = _usernameLogin.text;
            loginPanel.gameObject.SetActive(false);
            mainPanel.gameObject.SetActive(true);
        }
        else
        {
            //login fail
            ShowHint("登入失败");
            InitContentLogin();
        }
    }

    public void OnRegister(ReturnCode returnCode)
    {
        if (returnCode == ReturnCode.Success)
        {
            //verify successfully
            ShowHint("注册成功");
            loginPanel.gameObject.SetActive(false);
            loginPanel.gameObject.SetActive(true);
        }
        else
        {
            //login fail
            ShowHint("注册失败");
            InitContentRegister();
        }
    }
    
    public void OnLogOut(ReturnCode returnCode)
    {
        if (returnCode == ReturnCode.Success)
        {
            //verify successfully
            ShowHint("注销成功");
            mainPanel.gameObject.SetActive(false);
            loginPanel.gameObject.SetActive(true);
        }
        else
        {
            //login fail
            ShowHint("注销失败");
        }
    }
    
}
