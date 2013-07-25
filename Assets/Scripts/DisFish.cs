using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using System.Linq;

public class DisFish : MonoBehaviour {

	public static Transform Fish;
	public static Transform FishParent;
	
	// Dictionary < octant label, List<fish id> >
	public static M.OctIDDic OctFishDic = new M.OctIDDic();
	
	public static Queue FishDustbin = new Queue(); 
	
	// Dictionary < fish name, fish content >
	public static M.NameContBuf FishNameContBuf = new M.NameContBuf();
		
	void Start () {
		
	 	Fish = GameObject.Find("/fish1").transform;
		FishParent = GameObject.Find("/Fish").transform;
		
	}
	
	
	
	public static void FishDestroy()
	{
		if(FishDustbin.Count != 0)
		{
			string id = (string) FishDustbin.Dequeue();
			GameObject t = GameObject.Find("/Fish/"+id);
			if(!t)
			{
				print("Can't destroy fish with given id.");
			}
			Destroy( t );
		}
	}
	
	public static void FishInstantiate()
	{
		if(FishNameContBuf.IsEmpty() == false)
		{ 
			
			string namecontent = FishNameContBuf.Read();
			
			string [] split = namecontent.Split(new char [] {'|'},StringSplitOptions.RemoveEmptyEntries);
			
			if(split.Length<2)
				return;
			
			string name = split[0];
			string info = split[1];
			
			string n = M.GetLabelFromName(name);
			string id = M.GetIDFromName(name);
			
			if(n == null || id == null)
				return;
			if(OctFishDic.Contains(n, id)==true)
				return;
			//print("Render label: " + n + "    id: " + id);
			OctFishDic.Add(n,id);
			
			Transform oldfish = FishParent.Find(id);
			if(oldfish == null)
			{
				MakeFish(info);
			}
			else
			{
				MoveFish(oldfish, info);
			}
			
			
			
		}
	}
	
	public static void DeleteFishBySpace(List<string> octs)
	{
		if(octs.Count == 0)
			return;
		
		List<string> fishids;
		foreach(string o in octs)
		{
			if(OctFishDic.ContainsKey(o) == false)
			{
				continue;
			}
			
			fishids = OctFishDic.Get(o);
			foreach(string id in fishids)
			{
				if(id=="" && id==null)
					continue;
				
				FishDustbin.Enqueue(id);
				
			}
			OctFishDic.Remove(o);
		}
	}
	
	public static void AddFishBySpace(List<string> toadd)
	{
		if(toadd.Count == 0)
			return;
		
		foreach(string n in toadd)
		{
			if(OctFishDic.ContainsKey(n)==true && n!=Initialize.FirstAsteroidLabel)
			{
				continue;
			}
			
			string name = M.PREFIX + "/fish/octant/" + n + "/" + M.GetTimeComponent();
			RequestAll(name);
			OctFishDic.Add(n, null);
		}
	}
	
	public static void UpdateFishBySpace(List<string> nimbus)
	{
		if(nimbus.Count == 0)
			return;
		
		OctFishDic.Clear();
		AddFishBySpace(nimbus);
	}
	
	static Upcall.ccn_upcall_res DiscoverFishCallback (IntPtr selfp, Upcall.ccn_upcall_kind kind, IntPtr info)
	{
		//print("Fish Callback: " + kind);
		Egal.ccn_upcall_info Info = Egal.GetInfo(info);
		IntPtr h=Info.h;
		
		switch (kind) {
			case Upcall.ccn_upcall_kind.CCN_UPCALL_CONTENT_UNVERIFIED:
        	case Upcall.ccn_upcall_kind.CCN_UPCALL_CONTENT:
			
				string name = Egal.GetContentName(Info.content_ccnb);
				string content = Egal.GetContentValue(Info.content_ccnb, Info.pco); 
				FishNameContBuf.Write (name, content);
			
				string labels = M.GetLabelFromName(name);
				string oldcomponent = M.GetIDFromName(name);
				if(labels == null || oldcomponent == null) 
				{
					print("Ill name: " + name + ", belongs to: " + h);
					break;
				}
				 
				if(Discovery.nimbus.Contains(labels)==false) // we don't care about this octant any more
				{
					print("don't care: " + h + ", " + labels);
					break;
				}
			
				IntPtr c = Egal.ccn_charbuf_create();
				IntPtr templ = Egal.ccn_charbuf_create();
				IntPtr comp = Egal.ccn_charbuf_create();
            	Egal.ccn_name_init(c);
				Egal.ccn_name_init(comp);
				
				string matchedprefix = M.GetNameTillID(name);
				string [] split = matchedprefix.Split(new char [] {'/'},StringSplitOptions.RemoveEmptyEntries);
				foreach(string s in split)
				{
					Egal.ccn_name_append_str(c, s);
				}
			
				Egal.ccn_charbuf_append_tt(templ, (int)Dtag.ccn_dtag.CCN_DTAG_Interest, (int)TT.ccn_tt.CCN_DTAG);
				Egal.ccn_charbuf_append_tt(templ, (int)Dtag.ccn_dtag.CCN_DTAG_Name, (int)TT.ccn_tt.CCN_DTAG);
            	Egal.ccn_charbuf_append_closer(templ); // </Name> 
			
				Egal.ccn_charbuf_append_tt(templ, (int)Dtag.ccn_dtag.CCN_DTAG_Exclude, (int)TT.ccn_tt.CCN_DTAG);
			
				Egal.ccn_closure Selfp = (Egal.ccn_closure)Marshal.PtrToStructure(selfp, typeof(Egal.ccn_closure));
				M.Exclude Data = (M.Exclude) Marshal.PtrToStructure(Selfp.data, typeof(M.Exclude));	
				Data.filter = Data.filter + "," + oldcomponent;
				Marshal.StructureToPtr(Data, Selfp.data, true);
				Marshal.StructureToPtr(Selfp, selfp, true);
			
				string newfilterlist = Data.filter;
				
				string [] filters = newfilterlist.Split(new char [] {','},StringSplitOptions.RemoveEmptyEntries);
				foreach(string s in filters)
				{
					Egal.ccn_name_append_str(comp, s);
				}
				
				Egal.ccn_charbuf_append2(templ, comp);
			
				Egal.ccn_charbuf_append_closer(templ); // </Exclude>
				Egal.ccn_charbuf_append_closer(templ); // </Interest>
		
				// express interest again
				Handle.Pause();
				Egal.ccn_express_interest(h,c,selfp,templ);
				Handle.Resume();
				break;
			
			case Upcall.ccn_upcall_kind.CCN_UPCALL_FINAL:
				print("CCN_UPCALL_FINAL: " + h);
				
				break;
			case Upcall.ccn_upcall_kind.CCN_UPCALL_INTEREST_TIMED_OUT:
				print("CCN_UPCALL_INTEREST_TIMED_OUT: " + h);
				break;
			default: 
				print("othercallback: " + kind);
				break;
		}
		return Upcall.ccn_upcall_res.CCN_UPCALL_RESULT_OK;
	}
	
	public static void RequestAll(string name)
	{	
		M.Exclude Data = new M.Exclude();
		IntPtr pData = Marshal.AllocHGlobal(Marshal.SizeOf(Data));
		Marshal.StructureToPtr(Data, pData, true);
		
		//IntPtr ccn = Egal.GetHandle(); // connect to ccnd
		Handle.Pause();
		Egal.ExpressInterest(Handle.ccn, name, DiscoverFishCallback, pData, IntPtr.Zero); // express interest
		Handle.Resume();
	}
	
	public static Vector3 MakeFish(string info, bool activate = false)
	{
		Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(info);
		string id = values["callsign"];
		Vector3 position = M.GetGameCoordinates(values["lat"], values["lon"]);
		string label = M.GetLabel(position);
		
		Transform newFish = Instantiate(Fish, position, Quaternion.identity) as Transform;
		
		newFish.name = id;
		newFish.tag = "Fish";
		newFish.parent = FishParent;
//		newFish.Find("label").GetComponent<GUIText>().text = M.PREFIX + "/asteroid/" + id + "\n" 
//			+ M.PREFIX + "/asteroid/octant/" + label + "/" + id;
//		ControlLabels.ApplyAsteroidName(newAsteroid);
		
		if(activate == true)
		{
			//
		}
		return position;
	}
	
	public static void MoveFish(Transform fish, string info)
	{
		print("move fish! " + DateTime.Now.ToString("HH:mm:ss tt"));
		Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(info);
		Vector3 position = M.GetGameCoordinates(values["lat"], values["lon"]);
		fish.GetComponent<FishSwim>().target = position;
	}
}
