using UnityEngine;
using System.Collections;

public class TimerTestScript : MonoBehaviour
{

	// Drag the timer prefab from the inspector
	public Timer timer;
	// Variables to change the timer
	public int minutes;
	public int seconds;
	// Bool to timer can be started and stopped in the inspector
	public bool timerStarted = true;

	void Start ()
	{
		// Set the timer
		timer.SetTimer (minutes, seconds);
	}
	// Update is called once per frame
	void Update ()
	{
		// Do checks to see if the bool is false, and the timer has started
		if (!timerStarted && timer.isStarted)
		{
			timer.StopTimer ();
		}
        // Do checks to see if the bool is true, if the timer is stopped, and if the timer is not complete
        else if (timerStarted && !timer.isStarted && !timer.isComplete)
		{
			timer.StartTimer ();
		}
	}
}
