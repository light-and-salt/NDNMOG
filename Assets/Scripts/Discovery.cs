using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using System.Linq;

public class Discovery : MonoBehaviour {
	
	// List <octant labels>
	public static List<string> aura = null; 
	public static List<string> nimbus = null;
	public static List<string> oldnimbus = null;
	
	// boundary: 512*512*512
	public struct Boundary{
		public float xmin;
		public float xmax;
		public float ymin;
		public float ymax;
		public float zmin;
		public float zmax;
		public Boundary(float a, float b, float c, 
			float d, float e, float f)
		{
			xmin = a;
			xmax = b;
			ymin = c;
			ymax = d;
			zmin = e;
			zmax = f;
		}
	};
	static Boundary bry;
	
	IEnumerator Start () {
		
		aura = new List<string>();
		nimbus = new List<string>();
		bry = new Boundary(-1f, -1f, -1f, -1f, -1f, -1f);
		
		
		while(Initialize.finished != true)
		{
			yield return new WaitForSeconds(0.05f);
		}
		
		InvokeRepeating("CheckPos", 0, 0.5F); // player position changed
		InvokeRepeating("CheckEnv", 0, 60F); // environment changed
	}
	
	void Update()
	{
		DisAst.AstDestroy(); // destroy 0~1 asteroid per frame
		DisAst.AstInstantiate(); // instantiate 0~1 asteroid per frame
		
		DisFish.FishDestroy();
		DisFish.FishInstantiate();
	}
	
	void CheckPos() {
		
		if( InBound(transform.position) == false )
		{
			UpdateAuraNimbusBoundary();
			
			List<string> toadd = nimbus.Except(oldnimbus).ToList();
			List<string> todelete = oldnimbus.Except(nimbus).ToList();
			
			DisAst.DeleteAsteroidBySpace(todelete);
			DisAst.AddAsteroidBySpace(toadd);
			
			DisFish.DeleteFishBySpace(todelete);
			DisFish.AddFishBySpace(toadd);
			
			transform.Find("label").GetComponent<GUIText>().text = M.PREFIX + "/doll/zening\n" 
													+ M.PREFIX + "/doll/octant/" + aura[0] + "/zening";
			ControlLabels.ApplyOptions();
		}
		
    }
	
	static bool InBound(Vector3 position)
	{
		if(position.x<bry.xmin || position.y<bry.ymin || position.z<bry.zmin)
		{
			return false;
		}
		if(position.x>bry.xmax || position.y>bry.ymax || position.z>bry.zmax)
		{
			return false;
		}
		return true;
	}
	
	void UpdateAuraNimbusBoundary()
	{
		aura.Clear();
		string temp = M.GetLabel(transform.position);
		if(temp == null)
		{
			print("FindAsteroids.CheckPos(): Aura is null!");
			return;
		}
		aura.Add ( temp );
		bry = M.GetBoundaries(aura[0]); 
		
		oldnimbus = new List<string>(nimbus);
		nimbus.Clear();
		nimbus.AddRange( aura );
		nimbus.AddRange ( M.GetNeighbors(transform.position) );
	}
	
	void CheckEnv()
	{
		DisFish.UpdateFishBySpace(nimbus); // update fish position
	}
}
