using UnityEngine;
using UnityEngine.SceneManagement;

//Called by button components, used to load levels or quit the game
public class LevelLoad : MonoBehaviour
{
  public Pause pause;

  public void loadMainMenu() {loadScene(0);}
  public void loadLevel1() {loadScene(1);}
  public void loadLevel2() {loadScene(2);}
  public void loadLevel3() {loadScene(3);}
  public void loadLevel3_end() {loadScene(4);}
  
  public void loadScene(string name) {
    if (this.pause != null) this.pause.unpauseLoad();
    SceneManager.LoadScene(name);
  }

  public void loadScene(int index) {
    if (this.pause != null) this.pause.unpauseLoad();
    SceneManager.LoadScene(index);
  }

  public void quitGame() {Application.Quit();}
}
