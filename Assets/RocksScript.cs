using System;
using UnityEngine;

public class RocksScript : MonoBehaviour
{
   public void OnTriggerEnter(Collider other)
   {
      Debug.Log("Crashed into rocks. Game Over.");
   }
}
