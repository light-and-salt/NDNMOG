using System;
using UnityEngine;
using System.Collections;

public class DisPlayer : MonoBehaviour
{
	//no progress with building the bundle...still can't see other asteroids/NPCs using my own bundle.
	//gonna try something else.
	
	// Dictionary < octant label, List <player ids> >
	public static M.OctIDDic octAstPlayer = new M.OctIDDic();
	
	//delegate function can work with function pointer in C
	public delegate int testCallbackDelegate();
	
	int testCallback()
	{
		Debug.Log("zhehaoDebug : test callback executed.");
		return 1;
	}
	
	// Use this for initialization
	void Start ()
	{
		Debug.Log("Here");
		testCallbackDelegate h = new testCallbackDelegate(testCallback);
		//interestCallbackDelegate h1 = new interestCallbackDelegate(interestCallback);
		//IntPtr ptr = Egal.ccn_charbuf_create();
		Egal.zhehaoDebug(h);
		Egal.sync1();
		string myName = "zhehao";
		
		//I don't think we have mechanism for globally unique player id?
		//octAstPlayer.Add(myOct, myName);
		
		//zhehaoDebug is for testing callbacks
		
		//Egal.sync1();
		//make sure that threads created by sync1 is still running after sync1 finishes execution
		
		//question : how do I dump while executing bundle functions? Use log? I tried dumping to files but it doesn't work for now.
		//I have doubts with importing Egal.bundle...it doesn't seem to include my latest updates.
		//restarting Unity works now while 'reimporting' does update my changes.
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}

