using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
using ExitGames.Client.Photon;
using UnityEngine.UIElements;

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
        Instance = this;
        
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
        peer.Disconnect();
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
                OnHandleLogOutResponse(operationResponse);
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
    //call when client connection status change 
  
    public void OnStatusChanged(StatusCode statusCode)
    {
       
    }

    //call when server push a inform to client without request server
    //such as other clients need a request for sychroncizing
    public void OnEvent(EventData eventData)
    {
        throw new NotImplementedException();
    }
}
