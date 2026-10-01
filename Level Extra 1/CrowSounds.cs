using UnityEngine;
using System.Collections;

// Spawns a random crow sound close to the player
// Position relative to the player and frequency of crow sounds are random
public class CrowSounds : MonoBehaviour
{
  public Transform player;
  public AudioClip[] crowSounds;
  private AudioSource sound;

  void Start() {
    this.sound = this.gameObject.GetComponent<AudioSource>();
    StartCoroutine(crowNoises());
  }

  IEnumerator crowNoises() {
    while(true) {
      yield return new WaitForSeconds(Random.Range(6, 30));
      Vector3 playerPos = this.player.position;
      Vector3 soundPos = new Vector3(playerPos.x+Random.Range(-60, 60), playerPos.y+Random.Range(12, 60), playerPos.z+Random.Range(-60, 60));
      this.transform.position = soundPos;
      this.sound.clip = this.crowSounds[Random.Range(0, this.crowSounds.Length-1)];
      this.sound.Play();
    }
  }
}
