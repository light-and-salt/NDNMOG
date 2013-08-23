using UnityEngine;
using System.Collections;

public class NPC4_Control : MonoBehaviour {
		
	public string [] NPC4ChatMsg = {"Pouncy...Bouncy","Philosophically braindead","GUI.Box sucks","BBQ Chicken","You gonna fall","Unity crashes!"};
	
	public GUIText chatGuiText;
	public GUITexture chatGuiTexture;
	
	public int guiTime = 2;
	private Ray ray;
	
	public enum state
	{
		free,
		busy
	}
	
	public state status;
	
	public IEnumerator guiDisplay()
	{
		yield return new WaitForSeconds(guiTime);
		chatGuiText.gameObject.active = false;
		chatGuiTexture.gameObject.active = false;
		status = state.free;
	}
	
	public void showGUIText()
	{
		string chatMsg = NPC4ChatMsg[Random.Range(0,NPC4ChatMsg.Length)];
		Debug.Log(chatMsg);
		chatGuiText.text = chatMsg;
		
		Rect insetRect = chatGuiTexture.pixelInset;
		insetRect.x = Screen.width/2-10;
		insetRect.y = Screen.height/2-25;
		insetRect.width = -Screen.width/2-400+chatMsg.Length*8;
		insetRect.height = -Screen.height/2-150;
		//coordinates gained from testing results
		
		chatGuiTexture.pixelInset = insetRect;
		
		chatGuiText.gameObject.active = true;
		chatGuiTexture.gameObject.active = true;
		
		status = state.busy;
		
		StartCoroutine(guiDisplay());
	}
	
	// Use this for initialization
	void Start () {
		chatGuiText.gameObject.active = false;
		chatGuiTexture.gameObject.active = false;
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	public void OnMouseUp() 
	{
		switch (status)
		{
			case state.free:
			//freeInteraction();
			break;
			case state.busy:
			//busyInteraction();
			break;
		}
		Debug.Log("Up");
	}
	/*
	public void OnMouseEnter()
	{
		Debug.Log("Mouse enter");
	}
	
	public void OnMouseExit()
	{
		Debug.Log("Mouse exit");	
	}
	
	private void freeInteraction()
	{
		//status = state.busy;
		Debug.Log("Enter interaction");
	}
	
	private void busyInteraction()
	{
		Debug.Log("Busy now");
	}
	*/
}
