using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using System.Linq;

public class DisAst : MonoBehaviour {
	// DiscoverAsteroids
	
	// Dictionary < octant label, List <asteroid ids> >
	public static M.OctIDDic OctAstDic = new M.OctIDDic(); 
	
	// Dictionary < asteroid name, asteroid content >
	public static M.NameContBuf AstNameContBuf = new M.NameContBuf();
	
	// Queue< asteroid id >  asteroid to be deleted
	public static Queue AstDustbin = new Queue(); 
	
	
	public static Transform Tree2; // prefab for asteroids
	public static Transform AsteroidParent; // parent of asteroids
		
	void Start () {
			
		Tree2 = GameObject.Find("/tree2").transform;
		AsteroidParent = GameObject.Find("/Asteroid").transform;
		
	}
	
	
	public static void AstDestroy()
	{
		if(AstDustbin.Count != 0)
		{
			string id = (string) AstDustbin.Dequeue();
			GameObject t = GameObject.Find("/Asteroid/"+id);
			if(!t)
			{
				print("Can't destroy asteroid with given id.");
			}
			Destroy( t );
		}
			
	}
	
	public static void AstInstantiate()
	{
		if(AstNameContBuf.IsEmpty() == false)
		{ 
			
			string namecontent = AstNameContBuf.Read();
			
			string [] split = namecontent.Split(new char [] {'|'},StringSplitOptions.RemoveEmptyEntries);
			
			if(split.Length<2)
				return;
			
			string name = split[0];
			string info = split[1];
			
			
			{
				if(name == Initialize.FirstAsteroidName)
				{
					return;
				}
				
				string n = M.GetLabelFromName(name);
				string id = M.GetIDFromName(name);
				
				if(n == null || id == null)
					return;
				if(OctAstDic.Contains(n, id)==true)
					return;
				//print("Render label: " + n + "    id: " + id);
				OctAstDic.Add(n,id);
				
				MakeAnAsteroid(info);
			}
			
		}
		
	}

	public static void AddAsteroidBySpace(List<string> toadd)
	{
		if(toadd.Count == 0)
			return;
		
		foreach(string n in toadd)
		{
			if(OctAstDic.ContainsKey(n)==true && n!=Initialize.FirstAsteroidLabel)
			{
				//print("AddAsteroidBySpace(): this octant is not new! --" + n);
				continue;
			}
			RequestAll( M.PREFIX + "/asteroid/octant/" + n);
			OctAstDic.Add(n, null);
		}
	}
	
	
	public static void DeleteAsteroidBySpace(List<string> octs)
	{
		if(octs.Count == 0)
			return;
		
		List<string> asteroidids;
		foreach(string o in octs)
		{
			if(OctAstDic.ContainsKey(o) == false)
			{
				continue;
			}
			
			asteroidids = OctAstDic.Get(o);
			foreach(string id in asteroidids)
			{
				if(id=="" && id==null)
					continue;
				
				AstDustbin.Enqueue(id);
				
			}
			OctAstDic.Remove(o);
		}

	}
	
	
	static Upcall.ccn_upcall_res RequestAllCallback (IntPtr selfp, Upcall.ccn_upcall_kind kind, IntPtr info)
	{
		//print("RequestAllCallback: " + kind);
		Egal.ccn_upcall_info Info = Egal.GetInfo(info);
		IntPtr h=Info.h;
		
		switch (kind) {
			case Upcall.ccn_upcall_kind.CCN_UPCALL_CONTENT_UNVERIFIED:
        	case Upcall.ccn_upcall_kind.CCN_UPCALL_CONTENT:
			
				string name = Egal.GetContentName(Info.content_ccnb);
				string content = Egal.GetContentValue(Info.content_ccnb, Info.pco); 
				AstNameContBuf.Write (name, content);
			
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
					//TPool.AllHandles.Delete(h);
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
				//print("CCN_UPCALL_FINAL: " + h);
				
				break;
			case Upcall.ccn_upcall_kind.CCN_UPCALL_INTEREST_TIMED_OUT:
				//print("CCN_UPCALL_INTEREST_TIMED_OUT: " + h);
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
		Egal.ExpressInterest(Handle.ccn, name, RequestAllCallback, pData, IntPtr.Zero); // express interest
		Handle.Resume();
	}
	
	public static Vector3 MakeAnAsteroid(string info, bool activate = false)
	{
		Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(info);
		string id = values["fs"];
		Vector3 position = M.GetGameCoordinates(values["latitude"], values["longitude"]);
		string label = M.GetLabel(position);
		
		Transform newAsteroid = Instantiate(Tree2, position, Quaternion.identity) as Transform;
		
		newAsteroid.name = id;
		//newAsteroid.transform.localScale = new Vector3(1000f,1000f,1000f);
		newAsteroid.tag = "Asteroid";
		newAsteroid.parent = AsteroidParent;
		newAsteroid.Find("label").GetComponent<GUIText>().text = M.PREFIX + "/asteroid/" + id + "\n" 
			+ M.PREFIX + "/asteroid/octant/" + label + "/" + id;
		ControlLabels.ApplyAsteroidName(newAsteroid);
		
		if(activate == true)
		{
			newAsteroid.GetComponent<TreeScript>().Activate();
		}
		return position;
	}
	
	

}
