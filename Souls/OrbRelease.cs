using UnityEngine;

public class OrbRelease : MonoBehaviour
{
  public SoulsControl souls_control;
  public Orb orb;
  public MeshRenderer mesh;
  public Player player;
  public Transform player_t;

  void Update() {
    if (!this.mesh.isVisible) {return;}
    if (this.player.caught) {return;}
    if (Vector3.Distance(this.transform.position, this.player_t.position) > 2.5f) {return;}
    this.orb.levitateOrb();
    this.souls_control.releaseSoul();
  }
}
