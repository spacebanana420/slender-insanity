using UnityEngine;
using System.Collections;

public class LevelExtra1Start : MonoBehaviour
{
  public TextControl text;
  public AudioSource music;
  public Flashlight light;
  
  void Start() {
    StartCoroutine(levelStart());
    StartCoroutine(playMusic());
  }
  
  IEnumerator levelStart() {
    yield return new WaitForSeconds(1);
    this.light.turnOn();
    yield return new WaitForSeconds(2);
    text.displayTemporaryText("Abnormally-coloured pumpkins are scattered throughout the town\nFind and collect all 12 of them");
  }

  //Short introduction music
  IEnumerator playMusic() {
    yield return new WaitForSeconds(2);
    this.music.Play();
  }
}
