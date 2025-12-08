using UnityEngine;

//Sets which UI objects are supposed to be active and which are not
//Makes sure interface objects are correct regardless of what is done in the editor
public class InterfaceInit : MonoBehaviour
{
  
  public GameObject[] ui_enable;
  public GameObject[] ui_disable;
  
  void Start() {
    foreach (GameObject o in ui_enable) {o.active = true;}
    foreach (GameObject o in ui_disable) {o.active = false;}
  }
}
