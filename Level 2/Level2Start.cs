using UnityEngine;
using System.Collections;

public class Level2Start : MonoBehaviour
{
  public TextControl text;
  public AudioSource music;
  public Flashlight flashlight;
  private float start_volume;
  private bool fade_out = false;
  private bool fade_in = false;
  
  void Start() {
    this.start_volume = this.music.volume;
    this.music.volume = 0;
    StartCoroutine(levelStart());
  }

  void Update() {
    if (fade_in) {
      if (this.music.volume < this.start_volume) {
        this.music.volume += 0.1f * Time.deltaTime;
        return;
      }
      this.music.volume = this.start_volume;
      return;
    }
    if (fade_out) {
      if (this.music.volume > 0) {
        this.music.volume -= 0.05f * Time.deltaTime;
        return;
      }
      this.enabled = false;
      return;
    }
  }
  
  IEnumerator levelStart() {
    this.music.Play();
    this.fade_in = true;
    yield return new WaitForSeconds(1);
    this.flashlight.turnOn();
    yield return new WaitForSeconds(2);
    text.displayTemporaryText("Collect the 8 pages", 8);
    yield return new WaitForSeconds(12);
    this.fade_in = false;
    this.fade_out = true;
  }
}
