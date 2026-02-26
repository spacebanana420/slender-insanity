using UnityEngine;
using UnityEngine.SceneManagement;

//Called by button components, used to load levels or quit the game
public class LevelLoad : MonoBehaviour
{
  public Pause pause;

  public void loadMainMenu() {loadScene("Main Menu");}
  public void loadLevel1() {loadScene("Level 1");}
  public void loadLevel1_end() {loadScene("Level1Ending");}
  public void loadLevel2() {loadScene("Level2");}
  public void loadLevel3() {loadScene("Level3");}
  public void loadLevel3_end() {loadScene("Level3Ending");}

  public void reloadThisScene() {loadScene(SceneManager.GetActiveScene().buildIndex);}
  
  private void loadScene(string name) {
    if (this.pause != null) this.pause.unpauseLoad();
    SceneManager.LoadScene(name);
  }

  private void loadScene(int index) {
    if (this.pause != null) this.pause.unpauseLoad();
    SceneManager.LoadScene(index);
  }

  public void quitGame() {Application.Quit();}
}
