using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

//Animates the static effect and defines its transparency and volume
public class StaticEffect : MonoBehaviour
{
  public Material static_material;
  public AudioSource sound;
  public AudioSource sound_strong;

  //Pseudo-random offsets for the animation
  private float[] offsets_x = new float[]{0.3f, 1.2f, 0.75f, 1.7f};
  private float[] offsets_y = new float[]{0.2f, 1.45f, 1.1f, 0.3f};

  void Awake() {stop();}
  
  void Start() {
    this.sound.Play();
    this.sound_strong.Play();
    StartCoroutine(moveStatic());
  }

  public void stop() {
    this.static_material.color = new Color(1f, 1f, 1f, 0);
    this.sound.volume = 0;
    this.sound_strong.volume = 0;
  }

  //Sets static transparency and volume
  public void setStatic(float percentage) {
    float transparency = 0.6f * percentage; //Max transparency should be 0.5
    float volume = 0.2f * percentage; //Max volume should be 0.2
    this.static_material.color = new Color(1f, 1f, 1f, transparency);
    this.sound.volume = volume;
    this.sound_strong.volume = 0;
  }

  //Alternate mode for game over, uses more intense static (and eventually a different audio)
  public void setStatic_strong(float percentage) {
    float transparency = percentage;
    float volume1 = 0.2f * percentage; //Max volume should be 0.2
    float volume2 = 0.4f * percentage; //Max volume should be 0.2
    this.static_material.color = new Color(1f, 1f, 1f, transparency);
    this.sound.volume = volume1;
    this.sound_strong.volume = volume2;
  }

  //Pseudo-random static animation
  IEnumerator moveStatic() {
    int i = 0;
    int last_i = offsets_x.Length-1;
    while (true) {
      this.static_material.mainTextureOffset = new Vector2(offsets_x[i], offsets_y[i]);
      if (i == last_i){i = 0;}
      else{i++;}
      yield return new WaitForSeconds(0.05f);
    }
  }
}
