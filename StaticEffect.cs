using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

//Animates the static effect and defines its transparency and volume
public class StaticEffect : MonoBehaviour
{
  public Material static_material;
  public AudioSource static_sound;

  //Pseudo-random offsets for the animation
  private float[] offsets_x = new float[]{0.3f, 1.2f, 0.75f, 1.7f};
  private float[] offsets_y = new float[]{0.2f, 1.45f, 1.1f, 0.3f};
  
  void Start() {
    this.static_sound.Play();
    StartCoroutine(moveStatic());
  }

  public void stop() {
    this.static_material.color = new Color(1f, 1f, 1f, 0);
    this.static_sound.volume = 0;
  }

  //Sets static transparency and volume
  public void setStatic(float percentage) {
    float transparency = 0.5f * percentage; //Max transparency should be 0.5
    float volume = 0.2f * percentage; //Max volume should be 0.2
    this.static_material.color = new Color(1f, 1f, 1f, transparency);
    this.static_sound.volume = volume;
  }

  //Alternate mode for game over, uses more intense static (and eventually a different audio)
  public void setStatic_death(float percentage) {
    float transparency = percentage;
    float volume = 0.2f * percentage; //Max volume should be 0.2
    this.static_material.color = new Color(1f, 1f, 1f, transparency);
    this.static_sound.volume = volume;
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
