using UnityEngine;
using UnityEngine.SceneManagement;

//Called by button components, used to load levels or quit the game
public class LevelLoad : MonoBehaviour
{
  public void loadScene(string name) {SceneManager.LoadScene(name);}

  public void quitGame() {Application.Quit();}
}
