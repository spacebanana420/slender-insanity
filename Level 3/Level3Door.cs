using UnityEngine;

//In level 3, the player can exit the building after taking photographs of all ghosts
public class Level3Door : MonoBehaviour
{
  public Transform door;
  public Transform player;
  public Pause pause;
  public Level3Objective objective;
  
  void Update() {
    float distance = Vector3.Distance(this.player.position, door.position);
    if (distance > 3) return;
    this.pause.enabled = false;
    this.objective.triggerVictory();
    this.enabled = false;
  }
}
