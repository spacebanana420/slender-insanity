using UnityEngine;

//Sets which objects are supposed to be active on scene start and which are not
//Makes sure objects are correctly set up regardless of what is done in the editor
public class ObjectInit : MonoBehaviour
{
  
  public GameObject[] o_enable;
  public GameObject[] o_disable;
  
  void Awake() {
    foreach (GameObject o in o_enable) {o.active = true;}
    foreach (GameObject o in o_disable) {o.active = false;}
  }
}
