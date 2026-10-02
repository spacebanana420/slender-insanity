using UnityEngine;
using System.Collections;

// A secondary enemy seen in extra level 1
// Mostly harmless from a distance, but can kill the player if close enough
public class RoamGhost : MonoBehaviour
{
    public Player player;
    private SpriteAPI api;
    private AudioSource sound;

    private bool chasingPlayer = false;
    
    void Awake() {
      this.sound = this.gameObject.GetComponent<AudioSource>();
      this.api = this.gameObject.GetComponent<SpriteAPI>();

    }

    void Start() {
      this.api.enableBillboard();
      StartCoroutine(idle());
      StartCoroutine(chase());
    }
    
    //Moves to a random nearby place every few seconds
     IEnumerator idle() {
      while (true) {
        float moveDuration = Random.Range(2, 4);
        float waitDuration = Random.Range(3, 8);
        float elapsedTime = 0;
        float xSpeed = Random.Range(-3, 3);
        float zSpeed = Random.Range(-3, 3);
        Vector3 direction = new Vector3(xSpeed, 0, zSpeed);
        while (elapsedTime < moveDuration) {
          if (this.chasingPlayer || playerIsCaught()) { //Don't move randomly while chasing or if someone else caught the player
            yield return new WaitForSeconds(0.2f);
            continue;
          }
          
          this.api.moveToDirection(1, direction);
          elapsedTime += Time.deltaTime;
          yield return null;
        }
        yield return new WaitForSeconds(waitDuration);
      }
    }

    //Chases the player until it's far enough and enough time has passed
    IEnumerator chase() {
      while (true) {
        yield return null;
        if (this.api.getDistance() > 8) continue;

        float elapsedTime = 0;
        this.sound.Play();
        this.chasingPlayer = true;
        while (true) {
          if (playerIsCaught()){
            yield return new WaitForSeconds(0.2f);
            continue;
          }
          float playerDistance = this.api.getDistance();
          if (elapsedTime > 6 && playerDistance > 6) break;
          if (playerDistance < 2) break; //todo kill player
          this.api.move(4);
          elapsedTime += Time.deltaTime;
          yield return null;
        }
        this.chasingPlayer = false;
      }
    }

    private bool playerIsCaught() {return this.player.caught;}
}
