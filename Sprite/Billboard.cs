using UnityEngine;
using System.Collections;

//Simple billboard code to make an object stare at the player camera
//SpriteAPI can also run in billboard mode, but it does not have light and audio support
public class Billboard : MonoBehaviour
{
  public SpriteAPI spriteapi;
  public AudioSource sound;
  public Light light;
  
  private float fade_speed;
  private bool fade_out = false;
  private bool disable_on_fade = false;
  private bool sound_exists;
  private bool light_exists;

  void Awake() {
    this.sound_exists = this.sound != null;
    this.light_exists = this.light != null;
  }
  void Start() {this.spriteapi.enableBillboard();}
  
  public void fadeOut(float time = 4, bool disableObject = true) {
    StartCoroutine(fade(time, disableObject));
  }

  IEnumerator fade(float time, bool disableObject) {
    float spriteStep = this.spriteapi.getAlpha() / time;
    float soundStep = !this.sound_exists ? 0 : this.sound.volume / time;
    float lightStep = !this.light_exists ? 0 : this.light.intensity / time;

    while (true) {
      float alpha = this.spriteapi.getAlpha() - spriteStep * Time.deltaTime;
      
      this.spriteapi.setAlpha(alpha);
      bool allDone = alpha <= 0;
      if (this.sound_exists) {
        this.sound.volume -= soundStep * Time.deltaTime;
        allDone = allDone && this.sound.volume == 0;
      }
      if (this.light_exists) {
        this.light.intensity -= lightStep * Time.deltaTime;
        allDone = allDone && this.light.intensity == 0;
      }
      if (allDone) break;
      yield return null;
    }
    if (disableObject) this.gameObject.active = false;
  }
}
