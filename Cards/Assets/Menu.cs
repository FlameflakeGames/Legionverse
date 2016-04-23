using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour
{
	public GameObject[] scenes;
	public string[] sceneNames;
	public int screenState = 0;
	public Texture2D bg;
	public GUIStyle invisibleButton;

	void OnGUI()
	{
		if(screenState==0)
		{
			screenState=1;
			/*GUI.DrawTexture(new Rect((Screen.width/2)-500, 0, 1000, Screen.height), bg, ScaleMode.ScaleAndCrop);
			if(GUI.Button(new Rect((Screen.width/2)-64, (Screen.height/2)-32, 128, 64), "", invisibleButton))
			{
				screenState=1;
			}*/
		}
		if(screenState==1)
		{
			for(int i = 0; i<scenes.Length; i++)
			{
				if(GUI.Button(new Rect((Screen.width/2)-64, 128+(i*64), 128, 64), ""+sceneNames[i]))
				{
					this.gameObject.SetActive(false);
					scenes[i].SetActive(true);
				}
			}
		}
	}
}