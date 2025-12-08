using UnityEngine;
using UnityEngine.SceneManagement;

//Called by button components, used to load levels or quit the game
public class LevelLoad : MonoBehaviour
{
  public Pause pause;
  
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
