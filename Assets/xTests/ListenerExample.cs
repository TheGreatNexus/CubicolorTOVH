using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;

public class ListenerExample : MonoBehaviour {

	// Use this for initialization
	void Start () {
		EventManager.Instance.AddListener<MyFirstEvent>(MyFirstEventListener);
	}

	private void OnDestroy()
	{
		EventManager.Instance.RemoveListener<MyFirstEvent>(MyFirstEventListener);
	}

	void MyFirstEventListener(MyFirstEvent e)
	{
		Debug.Log(name + " received an event of type MyFirstEvent : "+e.eFloatRandomValue);
	}

}
