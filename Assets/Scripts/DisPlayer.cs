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
	
	public static M.cOctPlayerDic octPlayerList = new M.cOctPlayerDic ();
	public static testCallbackDelegate h;
	public static debugCallbackDelegate debugH;
	//delegate function works with function pointer in C
	public delegate int testCallbackDelegate (String octIndex,String setDifference);

	public delegate int debugCallbackDelegate (String debugStr);
	
	public const string prefixStr = "ccnx:/ndn/ucla.edu/apps/matryoshka/";
	public const string uniStrPrefix = "doll/name/";
	public const string syncStrPrefix = "doll/octant/";
	public const string posStrPrefix = "/position/";
	public const string initialPosStrPrefix = "init/";
	public static string debugFilePath = "";
	public static string myName;
	public const int nameLength = 8;
	
	//generate random my name using system.linq
	public static void generateRandomMyName ()
	{
		var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		var random = new System.Random ();
		myName = new string (Enumerable.Repeat (chars, nameLength).Select (s => s [random.Next (s.Length)]).ToArray ());
		debugFilePath = "log/debug-" + myName + ".txt";
	}
	
	public static int debugCallback ( String debugStr )
	{
		print ("debugCallback : " + debugStr);
		System.IO.File.AppendAllText (debugFilePath, "debugCallback : " + debugStr + "\n");
		return 1;
	}
	
	public static int syncOneCallback ( String octIndex, String setDifference )
	{
		try
		{
			if ( octIndex != null )
			{
				string [] names = setDifference.Split ('-');
				foreach (string s in names)
				{
					//why is there empty string in splitted names?
					if ( s != myName && s != null && s != "" )
					{
						string interestName = prefixStr + uniStrPrefix + s + posStrPrefix + initialPosStrPrefix;
						Egal.ExpressInterest (Handle.ccn, interestName, onReceiveSync2Data, IntPtr.Zero, IntPtr.Zero);
						System.IO.File.AppendAllText (debugFilePath, "myName : " + myName + ", received name : " + s + "\nExpressed interest name : " + interestName + "\n");
					}
				}
			}
			else
			{
				print ("Sync1 callback msg." + setDifference);
				System.IO.File.AppendAllText (debugFilePath, "Sync1 callback msg." + setDifference + "\n");
			}
		}
		catch (Exception e)
		{
			System.IO.File.AppendAllText (debugFilePath, "Error in sync1Callback" + e.Message);
		}
		return 1;
	}
	
	static Transform instantiateNewPlayer ( string name, Vector3 position )
	{
		try
		{
			Transform newPlayer = Instantiate (player, position, Quaternion.identity) as Transform;
			newPlayer.name = name;
			return newPlayer;
		}
		catch (Exception e)
		{
			System.IO.File.AppendAllText (debugFilePath, "Instantiate new player exception " + e.Message);
			return null;
		}
	}
	
	public static Vector3 getVector3FromStr ( string rString )
	{
		string[] temp = rString.Substring (1, rString.Length - 2).Split (',');
		float x = float.Parse (temp [0]);
		float y = float.Parse (temp [1]);
		float z = float.Parse (temp [2]);
		Vector3 rValue = new Vector3 (x, y, z);
		return rValue;
	}
	
	static Upcall.ccn_upcall_res onReceiveSync2Data ( IntPtr selfp, Upcall.ccn_upcall_kind kind, IntPtr info )
	{
		try
		{
			print ("sync2 " + kind);
			System.IO.File.AppendAllText (debugFilePath, "sync2 " + kind + "\n");
		
			Egal.ccn_upcall_info upcallInfo = (Egal.ccn_upcall_info)Marshal.PtrToStructure (info, typeof(Egal.ccn_upcall_info));
		
			//GetContentName results in an 'Argument cannot be null' error.
			//string outsInterestStr = Egal.GetContentName(upcallInfo.content_ccnb);
			
			switch (kind)
			{
				
			case Upcall.ccn_upcall_kind.CCN_UPCALL_CONTENT:
				string outsInterestStr = Egal.getInterestNameFromPI (info);
				string content = Egal.GetContentValue (upcallInfo.content_ccnb, upcallInfo.pco);
			
			//received string always looks like : name-octant-position-datetime
				System.IO.File.AppendAllText (debugFilePath, "Recved string:" + content + "\n");
				string [] splitStr = content.Split ('-');
				string playerName = splitStr [0];
				string playerOct = splitStr [1];
			
				Vector3 playerPos = getVector3FromStr (splitStr [2]);
				System.DateTime t = DateTime.Parse (splitStr [3]);
			
				M.cPlayerTime playerTime = octPlayerList.getPlayerTimeByName (playerOct, playerName);
				if ( playerTime == null )
				{
					Egal.octListAddName (playerOct, playerName);
					//instantiate a player and inserts him to the list
					M.cPlayerTime tempPT = new M.cPlayerTime (playerName, t, instantiateNewPlayer (playerName, playerPos));
				
					octPlayerList.Add (playerOct, tempPT);
					System.IO.File.AppendAllText (debugFilePath, "Player instantiated:" + playerName + "-" + playerOct + "-" + playerPos + "\n");
				}
				else
				{
					//received interest has a newer T
					if ( octPlayerList.updatePlayerTime (playerOct, playerTime, t) )
					{
						playerTime.setPos (playerPos);
					}
					System.IO.File.AppendAllText (debugFilePath, "Datetime and pos updated.\n");
				}
			//express interest with exclusion filter again ?
			
				string [] nameComponents = outsInterestStr.Split ('/');
				outsInterestStr = "";
				for (int i = 0; i< nameComponents.Length - 1; i++)
				{
					outsInterestStr += (nameComponents [i] + "/");
				}
			
				IntPtr nm = Egal.ccn_charbuf_create ();
				Egal.ccn_name_from_uri (nm, outsInterestStr);
				Egal.ccn_name_append_str (nm, splitStr [2] + "-" + splitStr [3]);
			
				Egal.ccn_closure Action = new Egal.ccn_closure (onReceiveSync2Data, IntPtr.Zero, 0);
				IntPtr pnt = Marshal.AllocHGlobal (Marshal.SizeOf (Action));
				Marshal.StructureToPtr (Action, pnt, true);
				
				//one second interval between two outstanding interest expressions
				System.Threading.Thread.Sleep(1000);
				Egal.ccn_express_interest (upcallInfo.h, nm, pnt, IntPtr.Zero);
			
				outsInterestStr += (splitStr [2] + "-" + splitStr [3]);
				System.IO.File.AppendAllText (debugFilePath, "outstanding interest string looks like : " + outsInterestStr + "\n");
			
			//could be something wrong while expressing interest? ExpressInterest encapsulation could be wrong?
			//other parts, like datetime-string conversion should be checked as well, plus sync2 logic needs to be double checked
			//the problem is that either position update or init triggers onReceiveSync2Interest, instead of both of them. 
			//Egal.ExpressInterest(Handle.sync2ccn, outsInterestStr, onReceiveSync2Data, IntPtr.Zero, IntPtr.Zero);
				break;
			case Upcall.ccn_upcall_kind.CCN_UPCALL_CONTENT_UNVERIFIED:
			
				break;
			case Upcall.ccn_upcall_kind.CCN_UPCALL_INTEREST_TIMED_OUT:
			
				break;
			case Upcall.ccn_upcall_kind.CCN_UPCALL_FINAL:
			
				break;
			}
		}
		catch (Exception e)
		{
			System.IO.File.AppendAllText (debugFilePath, "Error in onReceiveSync2Data" + e.Message);
				
		}
		return Upcall.ccn_upcall_res.CCN_UPCALL_RESULT_OK;
	}
	
	static Upcall.ccn_upcall_res onReceiveSync2Interest ( IntPtr selfp, Upcall.ccn_upcall_kind kind, IntPtr info )
	{
		try
		{
			System.IO.File.AppendAllText (debugFilePath, "Name " + myName + ", sync interest received: " + kind + "\n");
			//System.Threading.Thread.Sleep(1000);
			switch (kind)
			{
			case Upcall.ccn_upcall_kind.CCN_UPCALL_INTEREST:
				Egal.ccn_upcall_info upcallInfo = (Egal.ccn_upcall_info)Marshal.PtrToStructure (info, typeof(Egal.ccn_upcall_info));
				//fixed a silly bug caused by trying to extract 'info' from TIMED_OUT
				string s = Egal.getInterestNameFromPI (info);
				System.IO.File.AppendAllText (debugFilePath, "sync interest name received : " + s + "\nEnds.\n");
				print (s);
			
				if ( s.Contains (myName) && s.Contains ("position") )
				{
					if ( s.Contains ("init") )
					{
						string returnStr = myName + "-" + Discovery.aura [0] + "-" + GameObject.Find ("/player").transform.position + '-' + System.DateTime.Now.ToString ();
						//I failed when trying to use C# code to sign and return content...using C library code instead
						Egal.returnVerifiedStrContent (returnStr, info);
						System.IO.File.AppendAllText (debugFilePath, "ccn_put\n");
					}
					else
					{
					
						string tempStr = Egal.getNameLastStrComponent (info);
						System.IO.File.AppendAllText (debugFilePath, "parsed last name prefix received : " + tempStr + "\nEnds.\n");
			
						string [] posTimeStr = tempStr.Split ('-');
						
						Vector3 pos = getVector3FromStr (posTimeStr [0]);
						DateTime t = DateTime.Parse (posTimeStr [1]);
					
						if ( octPlayerList.getPlayerTimeByName (Discovery.aura [0], myName) != null )
						{
							string returnStr = myName + "-" + Discovery.aura [0] + "-" + GameObject.Find ("/player").transform.position + '-' + System.DateTime.Now.ToString ();
							Egal.returnVerifiedStrContent (returnStr, info);
						}
					}
				}
				break;
			case Upcall.ccn_upcall_kind.CCN_UPCALL_FINAL:
			
				break;
			}

		}
		catch (Exception e)
		{
			System.IO.File.AppendAllText (debugFilePath, "Error in onReceiveSync2Interest" + e.Message);
		}
		return Upcall.ccn_upcall_res.CCN_UPCALL_RESULT_OK;
	}
	
	public static void expressActionInterest ( string name )
	{
		//string interestName;
		
		//Egal.ExpressInterest(Handle.ccn, interestName, onReceiveSync2Data, IntPtr.Zero, IntPtr.Zero);
	}
	
	public static void expressPositionInterest ()
	{
		/*
		foreach (KeyValuePair<string,List<string>> entry in octPlayerList.getDic())
		{
			foreach (string entityName in entry.Value)
			{
				if (entityName != myName)
				{
					//Egal.ExpressInterest(Handle.ccn, 
				}
			}
		}
		*/
	}
	
	//test initiates sync1 actions.
	public static void test ()
	{	
		h = new testCallbackDelegate (syncOneCallback);
		debugH = new debugCallbackDelegate (debugCallback);
		//set interest filter for sync interests
		Egal.setSyncInterestFilter (Handle.ccn, h, prefixStr + syncStrPrefix, debugH);
		
		//set interest filter for unicast interests
		IntPtr name = Egal.ccn_charbuf_create ();
		string nameStr = prefixStr + uniStrPrefix;
		
		Egal.ccn_name_from_uri (name, nameStr);
		
		Egal.ccn_closure closure = new Egal.ccn_closure (onReceiveSync2Interest, IntPtr.Zero, 0);
		IntPtr pnt = Marshal.AllocHGlobal (Marshal.SizeOf (closure));
		Marshal.StructureToPtr (closure, pnt, true);
		
		Egal.ccn_set_interest_filter (Handle.ccn, name, pnt);
		System.IO.File.AppendAllText (debugFilePath, "Registered sync2 interest: " + prefixStr + uniStrPrefix + "\n");
	}
	
	public static void addPlayerBySpace ( List<String> toadd )
	{
		if ( toadd.Count == 0 )
		{
			return;
		}
		else
		{
			if ( octPlayerList.getPlayerTimeByName (Discovery.aura [0], myName) == null )
			{
				M.cPlayerTime tempPT = new M.cPlayerTime (myName, System.DateTime.Now, GameObject.Find ("/player/graphics").transform);
				print ("myName added." + Discovery.aura [0] + " " + myName);
				octPlayerList.Add (Discovery.aura [0], tempPT);
				Egal.octListAddNode (Discovery.aura [0]);
				Egal.octListAddName (Discovery.aura [0], myName);
			}
			foreach (String oct in toadd)
			{
				Egal.octListAddNode (oct);
				Egal.expressSyncInterest (oct, Handle.ccn, h);
				//expressPositionInterest();
			}
		}
	}
	
	//todelete remains unfinished now
	public static void deletePlayerBySpace ( List<String> todelete )
	{
		if ( todelete.Count == 0 )
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
		player = GameObject.Find ("/player/graphics").transform;
	}
}

