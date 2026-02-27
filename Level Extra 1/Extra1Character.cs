using UnityEngine;
using System.Collections;

//Contains the idle, movement and reaction logic for characters in extra level 1
//Supports various characters, each behaving a bit differently
//startle() is called by the player's camera
public class Extra1Character : MonoBehaviour
{
  public SpriteAPI api;
  public Texture[] spinSprites;
  public Texture surprisedSprite;

  public Character character;
  
  public enum Character {
    Yuyuko,
    Orin,
    Aya,
    Suwako
  }
  
  private Material material;
  private bool surprised = false;

  public void startle() {this.surprised = true;}
  
  void Awake() {
    this.material = this.api.mesh.material;
  }

  void Start() {
    this.api.enableBillboard();
    switch (this.character) {
      case Character.Yuyuko:
        StartCoroutine(idleAnimation(0.1f));
        break;
      case Character.Suwako:
        StartCoroutine(suwakoAnimation());
        break;
    }
  }

  //Generic, used with most characters
  IEnumerator idleAnimation(float animationSpeed) {
    int i = 0;
    while (!this.surprised) {
      while (this.api.getDistance() > 50) {yield return null;} //LOD-like optimisation, disable animation at distance
      this.material.mainTexture = this.spinSprites[i];
      i++;
      if (i == this.spinSprites.Length) i = 0;
      yield return new WaitForSeconds(animationSpeed);
    }
    switch (this.character) {
      case Character.Yuyuko:
        StartCoroutine(yuyukoStartle());
        break;
    }
  }

  //Yuyuko gets startled, reacts and flees
  IEnumerator yuyukoStartle() {
    this.material.mainTexture = this.surprisedSprite;
    yield return new WaitForSeconds(2);
    Vector3 fleeDirection = new Vector3(Random.Range(-10f, 10), 0, Random.Range(-10, 10));
    float timer = 0;
    while (timer < 10) {
      timer += 1 * Time.deltaTime;
      this.api.moveToDirection(3, fleeDirection);
      yield return null;
    }
    this.gameObject.active = false;
  }

  //Suwako gets surprised then jumps really high
  IEnumerator suwakoAnimation() {
    while (!this.surprised) {yield return null;}
    
    this.material.mainTexture = this.surprisedSprite;
    yield return new WaitForSeconds(2);
    float timer = 0;
    while (timer < 5) {
      timer += 1 * Time.deltaTime;
      this.api.move(0, 15);
      yield return null;
    }
    this.gameObject.active = false;
  }
}
