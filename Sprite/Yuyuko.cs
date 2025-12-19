using UnityEngine;

//Todo: fade-out when player gets too close
public class Yuyuko : MonoBehaviour
{
  public SpriteAPI spriteapi;

  void Update() {this.spriteapi.lookAtPlayer();}
}
