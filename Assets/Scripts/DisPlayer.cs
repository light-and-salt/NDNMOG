using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using System.Runtime.InteropServices;

public class DisPlayer : MonoBehaviour
{
	public static Transform player;
	
	// Dictionary < octant label, List <player ids> >
	// decided to take the 'sync1 keep a copy of the list' approach
	
	//Aug 21st : I'm curious why a first-time run does not work?
	
	public static M.OctIDDic octPlayerList = new M.OctIDDic();
	public static testCallbackDelegate h;
	//delegate function can work with function pointer in C
	public delegate int testCallbackDelegate(String octIndex, String setDifference);
	
	public const string prefixStr = "ccnx:/ndn/ucla.edu/apps/Matryoshka/";
	public const string registerStrPrefix = "doll/name/";
	
	public static string debugFilePath = "";
	
	public static string myName;
	public const int nameLength = 8;
	
	//generate random my name using system.linq
	public static void generateRandomMyName()
	{
		var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		var random = new System.Random();
		myName = new string(Enumerable.Repeat(chars, nameLength).Select(s => s[random.Next(s.Length)]).ToArray());
		debugFilePath = "log/debug-" + myName + ".log";
	}
	
	public static int syncOneCallback(String octIndex, String setDifference)
	{
		string [] names = setDifference.Split('-');
		foreach (string s in names)
		{
			//why is there empty string in splitted names?
			if (s!=myName && s!=null && s!="")
			{
				string interestName = prefixStr + registerStrPrefix + s + "/position";
				Egal.ExpressInterest(Handle.ccn, interestName, onReceiveSync2Data, IntPtr.Zero, IntPtr.Zero);
				Debug.Log(s + " ");
				
				System.IO.File.AppendAllText(debugFilePath,"myName : " + myName + ", received name : " + s + "\nExpressed interest name : " + interestName + "\n");
			}
		}
		return 1;
	}
	
	static int instantiateNewPlayer(string name, Vector3 position)
	{
		Transform newPlayer = Instantiate(player,position, Quaternion.identity) as Transform;
		newPlayer.name = name;
		return 1;
	}
	
	public static Vector3 getVector3FromStr(string rString)
	{
		string[] temp = rString.Substring(1,rString.Length-2).Split(',');
		float x = float.Parse(temp[0]);
		float y = float.Parse(temp[1]);
		float z = float.Parse(temp[2]);
	 	Vector3 rValue = new Vector3(x,y,z);
		return rValue;
	}
	
	static Upcall.ccn_upcall_res onReceiveSync2Data (IntPtr selfp, Upcall.ccn_upcall_kind kind, IntPtr info)
	{
		print ("sync2 " + kind);
		System.IO.File.AppendAllText(debugFilePath,"sync2 " + kind + "\n");
		switch (kind)
		{
		case Upcall.ccn_upcall_kind.CCN_UPCALL_CONTENT:
			Egal.ccn_upcall_info upcallInfo = (Egal.ccn_upcall_info)Marshal.PtrToStructure(info, typeof(Egal.ccn_upcall_info));
			string content = Egal.GetContentValue(upcallInfo.content_ccnb, upcallInfo.pco);
			System.IO.File.AppendAllText(debugFilePath,"Recved string:" + content + "\n");
			string [] splitStr = content.Split('-');
			string playerName = splitStr[0];
			string playerOct = splitStr[1];
			
			Vector3 playerPos = getVector3FromStr(splitStr[2]);
			
			if (octPlayerList.Contains(playerOct,playerName)==false)
			{
				octPlayerList.Add(playerOct,playerName);
				Egal.octListAddName(playerOct,playerName);
				instantiateNewPlayer(playerName, playerPos);
				System.IO.File.AppendAllText(debugFilePath,"Player instantiated:" + playerName + "-" + playerOct + "-" + playerPos + "\n");
			}
			else
			{
				
			}
			break;
		case Upcall.ccn_upcall_kind.CCN_UPCALL_CONTENT_UNVERIFIED:
			
			break;
		case Upcall.ccn_upcall_kind.CCN_UPCALL_INTEREST_TIMED_OUT:
			
			break;
		case Upcall.ccn_upcall_kind.CCN_UPCALL_FINAL:
			
			break;
		}
		return Upcall.ccn_upcall_res.CCN_UPCALL_RESULT_OK;
	}
	
	static Upcall.ccn_upcall_res onReceiveSync2Interest (IntPtr selfp, Upcall.ccn_upcall_kind kind, IntPtr info)
	{
		System.IO.File.AppendAllText(debugFilePath,"Name " + myName + ", sync interest received: " + kind + "\n");
		switch (kind)
		{
		case Upcall.ccn_upcall_kind.CCN_UPCALL_INTEREST:
			Egal.ccn_upcall_info upcallInfo = (Egal.ccn_upcall_info)Marshal.PtrToStructure(info, typeof(Egal.ccn_upcall_info));
			//upcallInfo.
			string s = Egal.getInterestNameFromPI(info);
			System.IO.File.AppendAllText(debugFilePath,"sync interest name received : " + s + "\nEnds.\n");
			print (s);
			//sign and return "confirmation " + myName in C#
			//need to add return myLoc, but cannot access transform in static.
			//s is the full name of the interest, so I use contains instead, which maybe dangerous if some very rare situations happen
			if (s.Contains(myName))
			{
				string returnStr = myName + "-" + Discovery.aura[0] + "/-" + GameObject.Find("/player").transform.position;
				//I failed when trying to use C# code to sign and return content...using C library code instead
				//remember : there's a trailing '/' behind discovery.aura
				Egal.returnVerifiedStrContent(returnStr , info);
				
				System.IO.File.AppendAllText(debugFilePath,"ccn_put\n");
			}
			break;
		case Upcall.ccn_upcall_kind.CCN_UPCALL_FINAL:
			
			break;
		}
		return Upcall.ccn_upcall_res.CCN_UPCALL_RESULT_OK;
	}
	
	public static void expressActionInterest(string name)
	{
		//string interestName;
		
		//Egal.ExpressInterest(Handle.ccn, interestName, onReceiveSync2Data, IntPtr.Zero, IntPtr.Zero);
	}
	
	public static void expressPositionInterest(string name)
	{
		foreach (KeyValuePair<string,List<string>> entry in octPlayerList.getDic())
		{
			foreach (string entityName in entry.Value)
			{
				if (entityName != myName)
				{
					
				}
			}
		}
		//string interestName = "ccnx:/ndn/ucla.edu/apps/Matryoshka/doll/" + ;
		//Egal.ExpressInterest(Handle.ccn, interestName, onReceiveSync2Data, IntPtr.Zero, IntPtr.Zero);
	}
	
	//test initiates sync1 actions.
	public static void test()
	{
		System.IO.File.AppendAllText(debugFilePath, "My name : " + myName+"\n");
			
		h = new testCallbackDelegate(syncOneCallback);
		//set interest filter for sync interests
		Egal.setSyncInterestFilter(Handle.ccn,h);
		
		//set interest filter for unicast interests
		IntPtr name = Egal.ccn_charbuf_create();
		string nameStr = prefixStr + registerStrPrefix;
		
		Egal.ccn_name_from_uri(name, nameStr);
		
		Egal.ccn_closure closure = new Egal.ccn_closure(onReceiveSync2Interest, IntPtr.Zero, 0);
		IntPtr pnt = Marshal.AllocHGlobal(Marshal.SizeOf(closure));
		Marshal.StructureToPtr(closure, pnt, true);
		
		Egal.ccn_set_interest_filter(Handle.ccn, name, pnt);
	}
	
	public static void addPlayerBySpace(List<String> toadd)
	{
		if (toadd.Count == 0)
		{
			return;
		}
		else
		{
			if (octPlayerList.Contains(Discovery.aura[0]+"/",myName) == false)
			{
				octPlayerList.Add(Discovery.aura[0]+"/",myName);
				Egal.octListAddNode(Discovery.aura[0]+"/");
				Egal.octListAddName(Discovery.aura[0]+"/", myName);
			}
			foreach (String oct in toadd)
			{
				Egal.expressSyncInterest(oct, Handle.ccn, h);
				expressPositionInterest(myName);
			}
		}
	}
	
	//todelete remains unfinished now
	public static void deletePlayerBySpace(List<String> todelete)
	{
		if (todelete.Count == 0)
		{
			return;
		}
		else
		{
			return;
		}
	}
	
	// Use this for initialization
	void Start ()
	{
		//using /player here causes Unity to crash, probably because of two CharacterControls? Stuff like that?
		player = GameObject.Find("/player/graphics").transform;
	}
}

