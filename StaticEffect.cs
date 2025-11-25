using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

//Change the tiling value of the Slender static image to make it look nice
public class StaticEffect : MonoBehaviour
{
  public Material static_material;
  
  private float[] offsets_x = new float[]{0.3f, 1.2f, 0.75f, 1.7f};
  private float[] offsets_y = new float[]{0.2f, 1.45f, 1.1f, 0.3f};
  
  void Start() {StartCoroutine(moveStatic());}

  //Intensity is a value between 0 and 1, modifies alpha channel
  public void setStaticIntensity(float intensity) {
    this.static_material.color = new Color(1f, 1f, 1f, intensity);
  }

  //Pseudo-random static animation
  IEnumerator moveStatic() {
    int i = 0;
    int last_i = offsets_x.Length-1;
    while (true) {
      this.static_material.mainTextureOffset = new Vector2(offsets_x[i], offsets_y[i]);
      if (i == last_i){i = 0;}
      else{i++;}
      yield return new WaitForSeconds(0.05f);
    }
  }
}
