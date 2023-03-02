using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class HideMessage : MonoBehaviour
{
  private void OnEnable()
  {
    //delay 2 seconds hide
    Invoke(nameof(HideSelf),2);
  }

  private void HideSelf()
  {
    GetComponentInChildren<Text>().text = "";
    gameObject.SetActive(false);
  }
}
