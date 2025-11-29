using UnityEngine;
using UnityEngine.InputSystem;
  
//Simple page collection code
public class Page : MonoBehaviour
{
  public PagesControl pages_control;
  public Transform player_t;
  public Player player;
  public MeshRenderer page_mesh;
  public AudioSource sound;

  void Update(){ //Rip OnMouseDown() you will be missed
    if (!this.page_mesh.isVisible) {return;}
    if (!Mouse.current.leftButton.wasPressedThisFrame) {return;}
    if (Vector3.Distance(this.transform.position, this.player_t.position) > 2) {return;}
    if (player.caught) {return;}
    this.sound.Play();
    this.pages_control.collectPage();
    this.page_mesh.enabled = false;
    this.enabled = false;
  }
}
