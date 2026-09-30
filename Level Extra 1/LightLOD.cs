using UnityEngine;

//LOD-like performance control for not rendering any light after a certain distance
//If the player is far, the lights won't render
//The light fades in and out smoothly to be discrete
public class LightLOD : MonoBehaviour
{
  public Transform player;
  public float maxDistance = 25;
  private Light light;
  private float maxIntensity; //Preserve original light intensity
  private float step; //How fast the light fades in/out

  void Awake(){
    this.light = this.gameObject.GetComponent<Light>();
    this.maxIntensity = this.light.intensity;
    this.step = this.maxIntensity * 1.8f;
  }
  void Update() {
    bool renderLight = getDistance() < this.maxDistance;
    float step = this.step * Time.deltaTime;
    if (!renderLight) step = -step;
    this.light.intensity = Mathf.Clamp(this.light.intensity+step, 0, 1);
    this.light.enabled = renderLight || this.light.intensity > 0;
  }
  private float getDistance() {return Vector3.Distance(this.transform.position, this.player.position);}
}
