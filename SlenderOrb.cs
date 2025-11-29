using UnityEngine;
using System.Collections;

//Pre-defined event for level 1 victory
//Slenderman jumpscares the player but does not kill him, revealing his "soul orb" instead
public class SlenderOrb : MonoBehaviour
{
  public Orb orb;
  public StaticEffect static_script;
  public AudioSource jumpscare;
  private float intensity;
  
  void Start() {
  }

  void Update() {
    if (this.intensity < 1) {
      this.static_script.setStatic_strong(this.intensity);
      this.intensity += 0.6f * Time.deltaTime;
      return;
    }
    //if ()
    //StartCoroutine(victoryEvent());
  }

  //IEnumerator victoryEvent() {
  //}
}
