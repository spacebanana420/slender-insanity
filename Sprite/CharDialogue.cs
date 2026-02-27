using UnityEngine;

//Class for storing character dialogue information
//Can be passed to TextControl.startSequence() to start a dialogue in which the character changes facial expressions
public class CharDialogue
{
  public string[] text;
  public bool hasFaces;
  private Material material;
  private Texture[] faces;
  private int[] face_index;
  
  public CharDialogue(string[] dialogue, Material material) {
    this.text = dialogue;
    this.material = material;
    this.faces = new Texture[dialogue.Length];
    this.hasFaces = true;
  }

  public CharDialogue(string[] dialogue) {this.text = dialogue;}

  //For the dialogue line at position "i", set a face to change to
  public void setFace(int i, Texture face) {this.faces[i] = face;}

  //Change the face during a dialogue for the line at position "i", used by TextControl automatically
  public void changeFace(int i) {
    if (this.faces[i] == null) return;
    this.material.mainTexture = this.faces[i];
  }
}
