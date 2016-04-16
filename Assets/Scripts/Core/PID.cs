/// <summary>
/// PID
/// http://www.codeproject.com/Articles/49548/Industrial-NET-PID-Controllers
/// </summary>

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Globalization;

public class PID
{	
	public delegate float Getfloat();
	public delegate void Setfloat(float value);

	[System.Serializable]
	public class InitParams
	{
		public InitParams() { }

		public float pG = 5f; 
		public float iG = 1f; 
		public float dG = .1f;
		public float oMin = -200f;
		public float oMax = 200f;
	}

	#region Fields
	
	//Gains
	private float kp;
	private float ki;
	private float kd;
	
	//Running Values
	//private float lastUpdate;
	//private float lastPV;
	private float errSum;
	private float lastErr;
	
	//Reading/Writing Values
	private Getfloat readPV;	// Process Variable
	private Getfloat readSP;	// Set point
	private Setfloat writeOV;	// Output variable
	
	//Max/Min Calculation
	private float outMax;
	private float outMin;
	
	//Threading and Timing
/*
 	private float computeHz = 1.0f;
	private Thread runThread;
*/

	#endregion
	
	#region Properties
	
	public float PGain
	{
		get { return kp; }
		set { kp = value; }
	}
	
	public float IGain
	{
		get { return ki; }
		set { ki = value; }
	}
	
	public float DGain
	{
		get { return kd; }
		set { kd = value; }
	}
	
	public float OutMin
	{
		get { return outMin; }
		set { outMin = value; }
	}
	
	public float OutMax
	{
		get { return outMax; }
		set { outMax = value; }
	}
	#endregion
	
	#region Construction / Deconstruction

	public PID(InitParams _params, Getfloat pvFunc, Getfloat spFunc, Setfloat outFunc)
	{
        AssignInit(_params);
		readPV = pvFunc;
		readSP = spFunc;
		writeOV = outFunc;
	}

	~PID()
	{
		//Disable();
		readPV = null;
		readSP = null;
		writeOV = null;
	}

    public void AssignInit(InitParams _params)
    {
        kp = _params.pG;
        ki = _params.iG;
        kd = _params.dG;
        outMax = _params.oMax;
        outMin = _params.oMin;
    }
	
	#endregion

	#region Public Methods

	public void Reset()
	{
		errSum = 0.0f;
		lastErr = 0.0f;
	}
	
	public void Compute()
	{
		if (readPV == null || readSP == null || writeOV == null)
			return;

		float sp = readSP();
		float pv = readPV();

		float err = sp - pv;
		errSum += err * Time.deltaTime;
		float errDiff = err - lastErr;
		lastErr = err;

		pv += (PGain * err * Time.deltaTime) + (IGain * errSum) + (DGain * errDiff);
		pv = Mathf.Clamp (pv, OutMin, OutMax);

		writeOV(pv);
	}
	
	#endregion
}