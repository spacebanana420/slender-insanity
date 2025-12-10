using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;


//Takes an ingame screenshot and saves it in the executable's working directory
public class Screenshooter : MonoBehaviour
{
  //Screenshot scale, integer value between 1 and +inf
  public int scale = 1;
  
  //When taking screenshots, Unity saves them in the Data directory instead of the executable's path
  //I use an absolute path to circumvent
  private string working_dir = Directory.GetCurrentDirectory();

  void Update() {
    if (!Keyboard.current.pKey.wasPressedThisFrame) {return;}
    long num = 0;
    string filename = this.working_dir+"/slender-insanity-0.png";
    while (File.Exists(filename)) {
      num+=1;
      filename = this.working_dir+"/slender-insanity-"+num+".png";
    }
    ScreenCapture.CaptureScreenshot(filename, this.scale);   
  }
}
