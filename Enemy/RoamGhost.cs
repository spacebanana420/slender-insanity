using UnityEngine;
using System.Collections;

// A secondary enemy seen in extra level 1
// Mostly harmless from a distance, but can kill the player if close enough
public class RoamGhost : MonoBehaviour
{
    public Player player;
    
    private SpriteAPI api;
    private AudioSource sound;
    private Vector3 originalPosition;
    private bool chasingPlayer = false;

    void Awake() {
      this.sound = this.gameObject.GetComponent<AudioSource>();
      this.api = this.gameObject.GetComponent<SpriteAPI>();
      this.originalPosition = this.transform.position;
    }

    void Start() {
      this.api.enableBillboard();
      StartCoroutine(idle());
      StartCoroutine(chase());
    }
    
    //Moves to a random nearby place every few seconds
     IEnumerator idle() {
      while (true) {
        if (this.api.getDistance(this.originalPosition) > 40) { //Too far, get back to spawn
          StartCoroutine(teleportToStart());
          yield return new WaitForSeconds(2); //The fade in + fade out time
        }
        float moveDuration = Random.Range(2, 4);
        float waitDuration = Random.Range(0, 0.5f);
        float elapsedTime = 0;
        float xSpeed = Random.Range(-2, 2);
        float zSpeed = Random.Range(-2, 2);
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

    //If the ghost strays too far from its spawn point, it teleports back to it 
    IEnumerator teleportToStart() {
      this.api.fadeOut(1);
      yield return new WaitForSeconds(1);
      this.api.teleportToPoint(this.originalPosition);
      this.api.fadeIn(1);
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
          if (elapsedTime > 6 && playerDistance > 8) break;
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
