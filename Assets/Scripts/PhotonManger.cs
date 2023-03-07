using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Common;
using UnityEngine;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;

public class PhotonManger : MonoBehaviour,IPhotonPeerListener
{
    public static PhotonManger Instance;
    
    //establish the link with given Server, create photon peer object
    private PhotonPeer peer;

    //Attribute
    public PhotonPeer Peer
    {
        get => peer;
        private set => peer = value;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if(Instance!=this)
        {
            Destroy(this.gameObject);
            return;
        }
        peer  = new PhotonPeer(this, ConnectionProtocol.Udp);
        peer.Connect("127.0.0.1:5055", "Demo1");
    }

    
    
    void temp()
    {
        //send an event to server
        //operation code (byte),operation parameters 

        Dictionary<byte, object> data = new Dictionary<byte, object>();
        data.Add(5,"Test--Game Development Class1 test this data");
        peer.OpCustom((byte) OperationCode.Login, data, true);
    }
    
    private void Update()
    {
        //establish listening Service with the Server
        peer.Service();
    }

    private void OnDestroy()
    {
        if(Instance==this)
        {
            if (PlayerController.Instance!=null)
            {
                Destroy(PlayerController.Instance.gameObject);                
            }
            
            peer.Disconnect();
        }
    }


    //call when the Server return a Debug inform
    public void DebugReturn(DebugLevel level, string message)
    {
       
    }

    //the client put a request positively
    public void OnOperationResponse(OperationResponse operationResponse)
    {
        
        // if (operationResponse.OperationCode.Equals(18))
        // {
        //     var data = operationResponse.Parameters;
        //     object str;
        //     if (data.TryGetValue(15,out str))
        //     {
        //         Debug.Log("Receive a response from Server, parse: "+str);
        //     }
        //     else
        //     {
        //         Debug.Log("Receive a response from Server but without data");
        //     }
        // }
        switch (operationResponse.OperationCode)
        {
            case (byte)OperationCode.Login:
                OnHandleLoginResponse(operationResponse);
                break;
            case (byte)OperationCode.Register:
                OnHandleRegisterResponse(operationResponse);
                break;
            case (byte)OperationCode.LogOut:
                OnHandleLogOutResponse(operationResponse);break;
                
            case (byte)OperationCode.SyncSpawnPlayer:
                OnHandleSyncSpawnPlayerResponse(operationResponse);
                break;
                
            case (byte)OperationCode.SyncPosInfo:
                OnHandleSyncPosInfoResponse(operationResponse);
                break;
            case (byte)OperationCode.SyncAttack:
                OnHandleSyncAttackResponse(operationResponse);
                break;
            default:break;
           
        }
    }

    private void OnHandleLoginResponse(OperationResponse operationResponse)
    {
        StartUIManger.Instance.OnLogin((ReturnCode)operationResponse.ReturnCode);
    }
    
    private void OnHandleRegisterResponse(OperationResponse operationResponse)
    {
        StartUIManger.Instance.OnRegister((ReturnCode)operationResponse.ReturnCode);
    }
    private void OnHandleLogOutResponse(OperationResponse operationResponse)
    {
        StartUIManger.Instance.OnLogOut((ReturnCode)operationResponse.ReturnCode);
    }
    
    
    private void OnHandleSyncSpawnPlayerResponse(OperationResponse operationResponse)
    {
        object usernameListObj;
        operationResponse.Parameters.TryGetValue((byte) ParameterCode.UsernameList, out usernameListObj);
        string usernameListString = usernameListObj.ToString();

        Debug.Log(usernameListObj);
        
        //Deserialization 

        using (StringReader reader = new StringReader(usernameListString))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<string>));
            
            //Deserialize 
            List<string> usernameList = (List<string>) serializer.Deserialize(reader);
            
            //Spawn other player in online
            Debug.Log(usernameList.Count);
            
            PlayerController.Instance.OnSpawnPlayerResponse(usernameList);
        }
    }


    private void OnHandleSyncPosInfoResponse(OperationResponse operationResponse)
    {
        
    }
    private void OnHandleSyncAttackResponse(OperationResponse operationResponse)
    {
        
    }
    //call when client connection status change 

    public void OnStatusChanged(StatusCode statusCode)
    {
       
    }

    //call when server push a inform to client without request server
    //such as other clients need a request for sychroncizing
    public void OnEvent(EventData eventData)
    {
        switch (eventData.Code)
        {
            case (byte)EventCode.SyncSpawnPlayer:
                OnSyncSpawnPlayerEvent(eventData);
                break;
            case (byte)EventCode.KickOut:
                OnKickOutEvent(eventData);
                break;
            case (byte)EventCode.ExitRoom:
                OnPlayerExitRoom(eventData);
                break;
        }
    }

    private void OnSyncSpawnPlayerEvent(EventData eventData)
    {
        object usernameObj;
        eventData.Parameters.TryGetValue((byte) EventCode.SyncSpawnPlayer,out usernameObj);
        
        //Spawn new onlined player after get the name
        PlayerController.Instance.OnSpawnPlayerEvent(usernameObj.ToString());
    }
    
    private void OnKickOutEvent(EventData eventData)
    {
       //this client is kicked out shift scene to Start
       SceneManager.LoadScene("Scenes/StartScene");
    }

    private void OnPlayerExitRoom(EventData eventData)
    {
        object usernameObj;
        eventData.Parameters.TryGetValue((byte) EventCode.SyncSpawnPlayer,out usernameObj);
        PlayerController.Instance.OnPlayerExitRoom(usernameObj.ToString());
    }
}
