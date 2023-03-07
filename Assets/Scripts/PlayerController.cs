using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
   public static PlayerController Instance;
   public GameObject player;
   public GameObject bulletPrefab;
   public GameObject playerPrefab;
   public Transform playerBox;

   public Button exitBtn;

   public float moveSpeed = 4;

   private void Awake()
   {
      Instance = this;
      
      //todo entry game
      PhotonManger.Instance.Peer.OpCustom((byte) OperationCode.EntryRoom, null, true);
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
      
      //notify server that you log in this game
      PhotonManger.Instance.Peer.OpCustom((byte) OperationCode.SyncSpawnPlayer, null, true);
      
      
      exitBtn.onClick.AddListener(() =>
      {
         SceneManager.LoadScene("StartScene");
      });
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
         GameObject.Instantiate(playerPrefab,playerBox).name= playerName;
      }
   }

   public void OnSpawnPlayerEvent(string playerName)
   {
      GameObject.Instantiate(playerPrefab,playerBox).name =playerName;
   }

   public void OnPlayerExitRoom(string playerName)
   {
      var find = playerBox.Find(playerName);
      if (find)
      {
         Destroy(find.gameObject);
      }
   }
}
