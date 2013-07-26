using UnityEngine;
using System.Collections;

public class NPC4_Control : MonoBehaviour {
		
	public string [] NPC4ChatMsg = {"Pouncy...Bouncy","Philosophically braindead","GUI.Box sucks","BBQ Chicken","You gonna fall","Unity crashes!"};
	
	public GUIText chatGuiText;
	//public GUITexture chatGuiTexture;
	public int guiTime = 2;
	
	public IEnumerator guiDisplay()
	{
		yield return new WaitForSeconds(guiTime);
		chatGuiText.gameObject.active = false;
		//guiTexture.gameObject.active = false;
	}
	
	public void showGUIText()
	{
		string chatMsg = NPC4ChatMsg[Random.Range(0,NPC4ChatMsg.Length)];
		Debug.Log(chatMsg);
		chatGuiText.text = chatMsg;
		chatGuiText.gameObject.active = true;
		//guiTexture.gameObject.active = true;
		StartCoroutine(guiDisplay());
	}
	
	// Use this for initialization
	void Start () {
		chatGuiText.gameObject.active = false;
		//guiTexture.gameObject.active = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
