using UnityEngine;

//Floating orb
public class Orb : MonoBehaviour
{
  public AudioSource sound;
  private float base_height;
  private float max_height;
  private bool levitate = false;
  private float levitate_speed = 0.05f;
  
  void Start() {
    this.base_height = this.transform.position.y;
    this.max_height = this.base_height+70;
  }

  void Update() {
    Vector3 pos = this.transform.position;
    if (!this.levitate) { //Idle
      pos.y = this.base_height + Mathf.Sin(Time.time * 0.5f) * 0.1f;
      this.transform.position = pos;
      return;
    }
    if (pos.y >= this.max_height) { //Disappear into the sky
      this.gameObject.active = false;
      return;
    }
    //Levitate
    pos.y += this.levitate_speed * Time.deltaTime;
    this.sound.pitch += 0.06f * Time.deltaTime;
    this.transform.position = pos;
    this.levitate_speed += 3.2f * Time.deltaTime;
    return;
  }
  
  public void levitateOrb() {this.levitate = true;}
}
