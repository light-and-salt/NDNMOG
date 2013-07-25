using UnityEngine;
using System.Collections;
using System;
using System.Threading;

public class Handle : MonoBehaviour {

	public static IntPtr ccn; // handle
    private static ManualResetEvent _pauseEvent = new ManualResetEvent(true);
    private static Thread _thread;
	//private static bool IsPlaying = true;
	
	void Start () {
		ccn = Egal.GetHandle(); 
		_thread = new Thread(Run);
        _thread.Start();
		
	}
	
	void OnApplicationQuit()
	{
		Resume();
		_thread.Abort();
		_thread.Join();
	}
	
	public void Run () 
	{
		while(_thread.IsAlive == true)
		{
			_pauseEvent.WaitOne(Timeout.Infinite);
            Egal.ccn_run(ccn, 20);
		}
		
	}
	
	public static void Pause()
    {
		//print("Pause()");
        _pauseEvent.Reset();
    }

    public static void Resume()
    {
		//print("Resume()");
        _pauseEvent.Set();
    }

	
}
