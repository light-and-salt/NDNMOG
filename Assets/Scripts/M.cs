using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;


public class M : MonoBehaviour {
	
	// M for Matryoshka
	// Here are some commonly used functions shared by all scripts
	// It's mostly about octree partitioning of the game world
	
	public static string PREFIX = "/ndn/ucla.edu/apps/matryoshka";
	
	public static string GetLabel(Vector3 position)
	{
		// decimal points in x,y,z
		// will not be used in this funciton
		
		// check if the point is in the game world
		if(InWorld(position) == false)
		{
			return null;
		}
		
		// get binaries
		string xbits = Convert.ToString((int)position.x, 2).PadLeft(16,'0');
		string ybits = Convert.ToString((int)position.y, 2).PadLeft(16,'0');
		string zbits = Convert.ToString((int)position.z, 2).PadLeft(16,'0');
		
		
		// reorganize
		string L1bits = ""+xbits[0] + ybits[0] + zbits[0]; 
		string L2bits = ""+xbits[1] + ybits[1] + zbits[1];
		string L3bits = ""+xbits[2] + ybits[2] + zbits[2];
		string L4bits = ""+xbits[3] + ybits[3] + zbits[3];
		string L5bits = ""+xbits[4] + ybits[4] + zbits[4];
		string L6bits = ""+xbits[5] + ybits[5] + zbits[5];
		string L7bits = ""+xbits[6] + ybits[6] + zbits[6];
		
		int temp1 = Convert.ToInt32(L1bits, 2); 
		int temp2 = Convert.ToInt32(L2bits, 2); 
		int temp3 = Convert.ToInt32(L3bits, 2); 
		int temp4 = Convert.ToInt32(L4bits, 2); 
		int temp5 = Convert.ToInt32(L5bits, 2); 
		int temp6 = Convert.ToInt32(L6bits, 2); 
		int temp7 = Convert.ToInt32(L7bits, 2); 
		
		string L1 = Convert.ToString(temp1, 8);
		string L2 = Convert.ToString(temp2, 8);
		string L3 = Convert.ToString(temp3, 8);
		string L4 = Convert.ToString(temp4, 8);
		string L5 = Convert.ToString(temp5, 8);
		string L6 = Convert.ToString(temp6, 8);
		string L7 = Convert.ToString(temp7, 8);
		
		string labels = ""+L1 + "/" + L2 + "/" + L3 + "/" + L4 + "/" + L5 + "/" + L6 + "/" + L7;
		
//		print(xbits);
//		print(ybits);
//		print(zbits);
		
		
		return labels;
	}
	
	public static Vector3 GetGameCoordinates(string str_lati, string str_longi)
	{
		// convert from latitude and longitude to game coordinates
		
		double radius = 30000;
		
		float latitude = Convert.ToSingle( str_lati );
		float longitude = Convert.ToSingle( str_longi ) ;
		float pi = 3.14159265359f;
		float theta = (float)(pi*(1.0/2.0 + latitude/180));
		float fi = (float)(pi*(longitude/180));
		double x = radius* 0.78*Mathf.Cos(theta/4)*Mathf.Sin(theta)*Mathf.Sin(fi);
		double y = radius*1.0*Mathf.Cos(theta);
        double z = radius* 0.78*Mathf.Cos(theta/4)*Mathf.Sin(theta)*Mathf.Cos(fi);

		// rotate the egg
		double xx = x;
		double yy = Mathf.Cos(pi/4)*y - Mathf.Sin(pi/4)*z;
		double zz = Mathf.Sin(pi/4)*y + Mathf.Cos(pi/4)*z;

		// translate the egg
		double xxx = xx + radius;
		double yyy = yy + radius;
		double zzz = zz + radius;

		Vector3 pos = new Vector3((float)xxx,(float)yyy,(float)zzz);

		return pos;
	}
	
	
	public static Discovery.Boundary GetBoundaries(string labels)
	{
		string [] split = labels.Split(new char [] {'/'},StringSplitOptions.RemoveEmptyEntries);
		
		int L1oct = Convert.ToInt32(split[0],8);
		int L2oct = Convert.ToInt32(split[1],8);
		int L3oct = Convert.ToInt32(split[2],8);
		int L4oct = Convert.ToInt32(split[3],8);
		int L5oct = Convert.ToInt32(split[4],8);
		int L6oct = Convert.ToInt32(split[5],8);
		int L7oct = Convert.ToInt32(split[6],8);
		
		string L1bits = Convert.ToString (L1oct,2).PadLeft(3,'0');
		string L2bits = Convert.ToString (L2oct,2).PadLeft(3,'0');
		string L3bits = Convert.ToString (L3oct,2).PadLeft(3,'0');
		string L4bits = Convert.ToString (L4oct,2).PadLeft(3,'0');
		string L5bits = Convert.ToString (L5oct,2).PadLeft(3,'0');
		string L6bits = Convert.ToString (L6oct,2).PadLeft(3,'0');
		string L7bits = Convert.ToString (L7oct,2).PadLeft(3,'0');
		
		string xbits = "" + L1bits[0] + L2bits[0] + L3bits[0] + L4bits[0] + L5bits[0] + L6bits[0] + L7bits[0];
		string ybits = "" + L1bits[1] + L2bits[1] + L3bits[1] + L4bits[1] + L5bits[1] + L6bits[1] + L7bits[1];
		string zbits = "" + L1bits[2] + L2bits[2] + L3bits[2] + L4bits[2] + L5bits[2] + L6bits[2] + L7bits[2];
		
		int x = Convert.ToInt32 (xbits,2);
		int y = Convert.ToInt32 (ybits,2);
		int z = Convert.ToInt32 (zbits,2);
		
		int xmin = x * 512; 
		int ymin = y * 512;
		int zmin = z * 512;
		
		int xmax = xmin + 512;
		int ymax = ymin + 512;
		int zmax = zmin + 512;
		
		
		Discovery.Boundary bry = new Discovery.Boundary(xmin, xmax, ymin, ymax, zmin, zmax);
		return bry;
		
	}
	
	public static List<string> GetNeighbors(Vector3 position)
	{
		List<string> neighborlist = new List<string>();
		int[,] neighbors = {{1,0,0}, {-1,0,0}, // x
							{0,1,0}, {0,-1,0}, // y
							{0,0,1}, {0,0,-1}, // z
							{1,1,0}, {1,-1,0}, {-1,1,0}, {-1,-1,0}, // x,y
							{1,0,1}, {1,0,-1}, {-1,0,1}, {-1,0,-1}, // x,z
							{0,1,1}, {0,1,-1}, {0,-1,1}, {0,-1,-1}, // y,z
							{1,1,1}, {-1,1,1}, {1,-1,1}, {1,1,-1}, {1,-1,-1}, {-1,1,-1}, {-1,-1,1},{-1,-1,-1} // x,y,z
		};
		
		Vector3 offset = new Vector3();
		string temp = null;
		for(int i = 0; i<26; i++)
		{
			offset.x = neighbors[i,0] * 512;
			offset.y = neighbors[i,1] * 512;
			offset.z = neighbors[i,2] * 512;
			
			temp = M.GetLabel(position+offset);
			if(temp!=null)
			{
				neighborlist.Add(temp);
			}
		}
		print(string.Join(",", neighborlist.ToArray())); // debug
		return neighborlist;
	}
	
	static bool InWorld(Vector3 position)
	{
		float worldsize = 65536; // 2^16
		
		if(position.x<0 || position.y<0 || position.z<0)
		{
			return false;
		}
		if(position.x>worldsize || position.y>worldsize || position.z>worldsize)
		{
			return false;
		}
		return true;
	}
	
	public static string GetLabelFromName(string name)
	{
		if(name.Contains("/octant/"))
		{
			int index = name.IndexOf("/octant/");
			
			if(name.Length<(index+21))
			{
				//print("Ill name: " + name);
				return null;
			}
			
			return name.Substring(index+8,13);
		}
		return null;
	}
	
	public static string GetIDFromName(string name)
	{
		if(name.Contains("/asteroid/octant/"))
		{
			int index = name.IndexOf("/octant/");
			if(name.Length<(index+22))
				return null;
			
			string tail = name.Substring(index + 22);
			string[] split = tail.Split(new char [] {'/'},StringSplitOptions.RemoveEmptyEntries);
			string id = split[0]; 
			
			return id;
		}
		
		if(name.Contains("/fish/octant/"))
		{
			int index = name.IndexOf("/octant/");
			if(name.Length<(index+51))
				return null;
			
			string tail = name.Substring(index + 51);
			string[] split = tail.Split(new char [] {'/'},StringSplitOptions.RemoveEmptyEntries);
			string id = split[0]; 
			
			return id;
		}
			
		return null;
	}
	
	public static string GetNameTillID(string name)
	{
		if(name.Contains("/asteroid/octant/"))
		{
			int index = name.IndexOf("/octant/");
			if(name.Length<(index+22))
				return null;
			
			string namebeforeid = name.Substring(0, index + 21);
			return namebeforeid;
		}
		
		if(name.Contains("/fish/octant/"))
		{
			int index = name.IndexOf("/octant/");
			if(name.Length<(index+51))
				return null;
			
			string namebeforeid = name.Substring(0, index + 50);
			return namebeforeid;
		}
		
		return null;
	}
	
	public static string GetTimeComponent(int addhour = -3, int addmin = -15)
	{
		DateTime ct = DateTime.Now.AddMinutes(addmin);
		ct = ct.AddHours(addhour);
		//DateTime ct = DateTime.Now.AddMinutes(2);
		string component = ct.ToString("ddd-MMM-dd-HH.mm") + ".00-PDT-" + ct.ToString("yyyy");
		return component;
	}
	
	public class NameContBuf
	{
		private Queue buf = new Queue ();
		private static ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();
		
		public string Read()
		{
			string item;
			
			rwl.EnterWriteLock();
			item = (string)buf.Dequeue();
			rwl.ExitWriteLock();
         		
      		return item;
		}
		
		public void Write(string name, string content)
		{
			rwl.EnterWriteLock();
			buf.Enqueue ("" + name + "|" + content);
      		rwl.ExitWriteLock();
		}
		
		public bool IsEmpty()
		{
			if(buf.Count == 0)
				return true;
			else
				return false;
		}
		
		
	}
	
	public class OctIDDic
	{
		// dictionary < oct label, list<id> >
		private static Dictionary<string,List<string>> dic = new Dictionary<string, List<string>>();
		
		public void Add(string oct, string id)
		{
			if(oct==null || oct=="")
				return;
	
			if( (id == null || id == "") && dic.ContainsKey(oct)==false)
			{
				dic.Add (oct,new List<string>());
				return;
			}
			
			if( id != null && id != "" && dic.ContainsKey(oct)==false)
			{
				dic.Add (oct,new List<string>());
				dic[oct].Add(id);
				return;
			}
			
			if( id != null && id != "" && dic.ContainsKey(oct)==true)
			{
				dic[oct].Add(id);
				return;
			}
			
		
		}
		
		public void Add(string name)
		{
			string oct = M.GetLabelFromName(name);
			string id = M.GetIDFromName(name);
			
			Add(oct, id);
		}
		
		public bool Contains(string oct, string id)
		{
			if(dic.ContainsKey(oct)==true)
			{
				if(dic[oct].Contains(id))
				{
					return true;
				}
			}
			return false;
		}
		
		public bool ContainsKey(string oct)
		{
			return dic.ContainsKey(oct);
		}
		
		public List<string> Get(string oct)
		{
			return dic[oct];
		}
		
		public void Remove(string oct)
		{
			dic.Remove(oct);
		}
		
		public int Count()
		{
			return dic.Count;
		}
		
		public void Clear()
		{
			dic.Clear();
		}
	}
	
	public struct Exclude
	{
		public string filter; // components, seperated by ','
	}
	
}
