using UnityEngine;

//Used in main menu, chooses whether to show Slenderman looking at the camera or someone else (easter egg)
public class EnemySelect : MonoBehaviour
{
  public GameObject slender;
  public GameObject easteregg;

  //5% probability of easter egg taking Slenderman's place
  void Awake() {
    float result = Random.value;
    if (result <= 0.98f) slender.active = true;
    else easteregg.active = true;
  }
}
