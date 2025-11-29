using UnityEngine;
using System.Collections;

//Pre-defined event for level 1 victory
//Slenderman jumpscares the player but does not kill him, revealing his "soul orb" instead
public class SlenderOrb : MonoBehaviour
{
  public GameObject slender;
  public Orb orb_script;
  public GameObject orb;
  public StaticEffect static_script;
  public BlankScreen blank_screen;
  public AudioSource jumpscare;
  public AudioSource thunder;
  
  void Start() {StartCoroutine(victoryEvent());}

  // void Update() {
  //   if (this.intensity < 1) {
  //     this.static_script.setStatic_strong(this.intensity);
  //     this.intensity += 0.6f * Time.deltaTime;
  //     return;
  //   }
  //   this.static_script.setStatic_strong(0);
  //   //if ()
  //   //StartCoroutine(victoryEvent());
  // }

  IEnumerator victoryEvent() {
    float intensity = 0;
    this.jumpscare.Play();
    while (intensity < 1) {
      this.static_script.setStatic_strong(intensity);
      intensity += 0.6f * Time.deltaTime;
      yield return new WaitForSeconds(0.005f);
    }
    this.slender.active = false;
    this.orb.active = true;
    this.thunder.Play();
    this.static_script.setStatic_strong(0);
    this.blank_screen.displayWhiteScreen();
    this.blank_screen.fadeFromWhite(4);
    yield return new WaitForSeconds(10);
    this.orb_script.levitateOrb();
    yield return new WaitForSeconds(10);
    this.blank_screen.displayBlackScreen();
  }
}
