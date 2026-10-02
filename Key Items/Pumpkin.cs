using UnityEngine;
using UnityEngine.InputSystem;
  
//Pumpkin collection code for extra level 1
public class Pumpkin : MonoBehaviour
{
  public PumpkinControl pumpkin_control;
  public Player player;
  public GameObject light;

  private Transform player_transform;
  private MeshRenderer mesh;
  private AudioSource sound;

  void Awake() {
    this.player_transform = this.player.gameObject.GetComponent<Transform>();
    this.mesh = this.gameObject.GetComponent<MeshRenderer>();
    this.sound = this.gameObject.GetComponent<AudioSource>();
  }

  void Update() {
    if (!this.mesh.isVisible) return;
    if (!Mouse.current.leftButton.wasPressedThisFrame) return;
    if (Vector3.Distance(this.transform.position, this.player_transform.position) > 2) return;
    if (player.caught) return;
    
    if (this.sound != null) this.sound.Play();
    this.pumpkin_control.collectPumpkin();
    this.mesh.enabled = false;
    this.light.active = false;
    this.enabled = false;
  }
}
