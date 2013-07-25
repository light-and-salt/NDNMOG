using UnityEngine;
using System.Collections;

public class ControlLabels : MonoBehaviour {

	public enum Option {none, self, aura, nimbus};
	public static int Show;
	
	
	private static Transform asteroidparent;
	private static Transform player;
	
	void Start()
	{
		asteroidparent = GameObject.Find("/Asteroid").transform;
		player = GameObject.Find("/player").transform;
		
		Show = (int)Option.self;
		ApplySelf();
		
		
	}
	
	void Update () {
		if(Input.GetKeyUp(KeyCode.N))
		{
			Show = GetNextOption();
			ApplyOptions();
		}
		
	}
	
	
	int GetNextOption()
	{
		int nextoption;
		if(Show != (int)Option.nimbus)
		{
			nextoption = Show+1;
		}
		else
		{
			nextoption = (int)Option.none;
		}
		return nextoption;
	}
	
	public static void ApplyOptions()
	{
		switch(Show)
			{
			case (int)Option.none:
				ApplyNone(); break;
			case (int)Option.self:
				ApplySelf(); break;
			case (int)Option.aura:
				ApplyAura(); break;
			case (int)Option.nimbus:
				ApplyNimbus(); break;
			}
	}
	
	static void ApplyNone()
	{
		print("Show label for none.");
		// player
		player.Find("label").gameObject.SetActiveRecursively(false);

		// asteroids
		foreach(Transform t in asteroidparent)
		{
			t.Find("label").gameObject.SetActiveRecursively(false);
		}
		
	}
	
	static void ApplySelf()
	{
		print("Show label for self.");
		// player
		player.Find("label").gameObject.SetActiveRecursively(true);
		
		// asteroids
		foreach(Transform t in asteroidparent)
		{
			t.Find("label").gameObject.SetActiveRecursively(false);
		}
	}
	
	static void ApplyAura()
	{
		print("Show labels for aura.");
		// player
		player.Find("label").gameObject.SetActiveRecursively(true);
		
		// asteroids
		foreach(Transform t in asteroidparent)
		{
			if( IsAsteroidInAura(t.name) == true)
			{
				t.Find("label").gameObject.SetActiveRecursively(true);
			}
			else
			{
				t.Find("label").gameObject.SetActiveRecursively(false);
			}
		}
	}
	
	static bool IsAsteroidInAura(string id)
	{
		if(Discovery.aura == null || DisAst.OctAstDic.Count() == 0)
			return false;
		string aura = Discovery.aura[0];
		return DisAst.OctAstDic.Contains(aura, id);
	}
	
	static void ApplyNimbus()
	{
		print("Show labels for nimbus");
		// player
		player.Find("label").gameObject.SetActiveRecursively(true);
		
		// asteroids
		foreach(Transform t in asteroidparent)
		{
			t.Find("label").gameObject.SetActiveRecursively(true);
		}
	}
	
	public static void ApplyAsteroidName(Transform obj) // set label for asteroid
	{
		switch(Show)
			{
			case (int)Option.none:
			obj.Find("label").gameObject.SetActiveRecursively(false);
			break;
			case (int)Option.self:
			obj.Find("label").gameObject.SetActiveRecursively(false);
			break;
			case (int)Option.aura:
			if( IsAsteroidInAura(obj.name) == true)
			{
				obj.Find("label").gameObject.SetActiveRecursively(true);
			}
			else
			{
				obj.Find("label").gameObject.SetActiveRecursively(false);
			} 
			break;
			case (int)Option.nimbus:
			obj.Find("label").gameObject.SetActiveRecursively(true);
			break;
			}
	}
}
