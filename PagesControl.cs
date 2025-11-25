using UnityEngine;

//Controls difficulty according to the number of collected pages
public class PagesControl : MonoBehaviour
{
  public GameObject slenderman;
  public Slenderman slender_script;
  public byte pages_collected = 0;
  private bool first_page = true;

  //Slenderman's stats, difficulty adjustment
  //One value for each page collected (1 to 7 pages)
  public float[] speeds = {0.5f, 1, 2, 3, 4, 5, 5.5f};
  public float[] look_limits = {10, 9, 8, 7, 6, 5, 4};
  public float[] teleport_limits = {60, 50, 40, 30, 25, 20, 15};
  public float[] invisible_limits = {80, 90, 100, 110, 120, 120, 120};
  public bool[] can_be_invisible = {true, true, true, true, true, false, false};

  
  public void collectPage() {
    int i = this.pages_collected;
    this.pages_collected += 1;
    this.slender_script.speed = this.speeds[i];
    this.slender_script.look_limit = this.look_limits[i];
    this.slender_script.teleport_limit = this.teleport_limits[i];
    this.slender_script.invisible_limit = this.invisible_limits[i];
    this.slender_script.can_be_invisible = this.can_be_invisible[i];
    if (first_page) {
      this.slenderman.active = true;
      this.first_page = false;
    }    
  }
}
