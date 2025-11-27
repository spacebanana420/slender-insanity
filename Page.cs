using UnityEngine;
using UnityEngine.InputSystem;
  
//Simple page collection code
public class Page : MonoBehaviour
{
  public PagesControl pages_control;
  public Transform player;
  public MeshRenderer page_mesh;
  public AudioSource sound;

  void Update(){ //Rip OnMouseDown() you will be missed
    if (!page_mesh.isVisible) {return;}
    if (!Mouse.current.leftButton.wasPressedThisFrame) {return;}
    if (Vector3.Distance(this.transform.position, player.position) > 2) {return;}
    sound.Play();
    pages_control.collectPage();
    this.page_mesh.enabled = false;
    this.enabled = false;
  }
}
