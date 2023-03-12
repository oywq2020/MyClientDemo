using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Common;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class PlayerController : MonoBehaviour
{
   public static PlayerController Instance;
   public GameObject player;
   public GameObject bulletPrefab;
   public GameObject playerPrefab;
   public Transform playerBox;

   public Button exitBtn;

   public float moveSpeed = 4;

   private Vector3 _lastPosition = new Vector3(0, 1, 0);
   private Quaternion _lastRotation = Quaternion.identity;

   private Dictionary<string, GameObject> _currentPlayerDic = new Dictionary<string, GameObject>();

   private void Awake()
   {
      Instance = this;
   }

   private void OnDestroy()
   {
      Debug.Log("Destroy Main Scene");
      //todo entry
      PhotonManger.Instance.Peer.OpCustom((byte) OperationCode.ExitRoom, null, true);
      
   }

   private void Start()
   {
      player.GetComponent<MeshRenderer>().material.color = Color.green;
      //todo entry game
      PhotonManger.Instance.Peer.OpCustom((byte) OperationCode.EntryRoom, null, true);
      //notify server that you log in this game
      PhotonManger.Instance.Peer.OpCustom((byte) OperationCode.SyncSpawnPlayer, null, true);
      
      //Event
      exitBtn.onClick.AddListener(() =>
      {
         SceneManager.LoadScene("StartScene");
      });
      
      
      //sync position
      InvokeRepeating(nameof(SyncPosRot),2,0.01f); //sync 20 times per second
   }

   //sync position to other palyer
   void SyncPosRot()
   {
      //SyncPos
      if (Vector3.Distance(player.transform.position,_lastPosition)>0.1)
      {
         _lastPosition = player.transform.position;
         Vector3Data vector3Data = new Vector3Data();
         vector3Data.x = _lastPosition.x;
         vector3Data.y = _lastPosition.y;
         vector3Data.z = _lastPosition.z;
         
         using (StringWriter sw = new StringWriter())
         {
            //create serialization object
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Vector3Data));
            //use this object to serialize
            xmlSerializer.Serialize(sw, vector3Data);

            //transform serialized xml to string
            string vector3DataString = sw.ToString();
            
            Dictionary<byte, object> data = new Dictionary<byte, object>();
            data.Add((byte)ParameterCode.PositionInfo, vector3DataString);
            
            //send current pos for synchronizing for other player
            PhotonManger.Instance.Peer.OpCustom((byte) OperationCode.SyncPosInfo, data, true);
         }
      }

      // ReSharper disable once RedundantCheckBeforeAssignment
      if (player.transform.rotation!=_lastRotation)
      {
         //SyncRotation 
         _lastRotation = player.transform.rotation;
         Vector3Data vector3Data = new Vector3Data();
         vector3Data.x = _lastRotation.eulerAngles.x;
         vector3Data.y = _lastRotation.eulerAngles.y;
         vector3Data.z = _lastRotation.eulerAngles.z;
         
         using (StringWriter sw = new StringWriter())
         {
            //create serialization object
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Vector3Data));
            //use this object to serialize
            xmlSerializer.Serialize(sw, vector3Data);

            //transform serialized xml to string
            string vector3DataString = sw.ToString();
            
            Dictionary<byte, object> data = new Dictionary<byte, object>();
            data.Add((byte)ParameterCode.RotationInfo, vector3DataString);
            
            //send current pos for synchronizing for other player
            PhotonManger.Instance.Peer.OpCustom((byte) OperationCode.SyncRotInfo, data, true);
         }
      }
   }

   
   
   private void Update()
   {
      player. transform.Translate(new Vector3(0,0,Input.GetAxis("Vertical"))*moveSpeed*Time.deltaTime);
      player.transform.Rotate(new Vector3(0,Input.GetAxis("Horizontal"),0),Space.Self);

      if (Input.GetKeyDown(KeyCode.Space))
      {
         GameObject go = GameObject.Instantiate(bulletPrefab,
            player.transform.Find("Gun").position,Quaternion.identity);
         
         go.GetComponent<Rigidbody>().velocity = player.transform.forward*40;
         
         Destroy(go,1.5f);
      }
   }

   public void OnSpawnPlayerResponse(List<string> usernameList)
   {
      foreach (var playerName in usernameList)
      {
         var others = Instantiate(playerPrefab,playerBox);
         others.name = playerName;
         _currentPlayerDic.Add(playerName,others);
      }
   }

   public void OnSpawnPlayerEvent(string playerName)
   {
     var others = Instantiate(playerPrefab,playerBox);
         others.name = playerName;
         _currentPlayerDic.Add(playerName,others);
   }

   public void OnPlayerExitRoom(string playerName)
   {
      if (_currentPlayerDic.TryGetValue(playerName,out GameObject other))
      {
         _currentPlayerDic.Remove(playerName);
         Destroy(other.gameObject);
      }
   }

   public void OnSyncPosRotEvent(List<PlayerData> playerDataList)
   {
      foreach (var playerData in playerDataList)
      {
         if (_currentPlayerDic.TryGetValue(playerData.Username,out GameObject other))
         {
            other .transform.position =
               new Vector3(playerData.Pos.x,playerData.Pos.y,playerData.Pos.z) ;
            other.transform.rotation = Quaternion.Euler(playerData.Rot.x,playerData.Rot.y,playerData.Rot.z);
         }
      }
   }
}
