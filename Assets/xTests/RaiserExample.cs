using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;

public class RaiserExample : MonoBehaviour {

	private void OnGUI()
	{
		if(GUI.Button(new Rect(10,10,200,200)," Raise\nMyFirstEvent\nevent"))
		{
			EventManager.Instance.Raise(new MyFirstEvent() { eFloatRandomValue=Random.value});
		}
	}
}
