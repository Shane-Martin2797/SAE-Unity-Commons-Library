using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour {

	// Store seconds and minutes in ints
	private int seconds;
	private int minutes;
	// Use a float to store a time variable
	// This variable will be used to make the time go down
	private float gameTimer;
    // Bools for starting and completing the timer, can only be set in this script
    public bool isStarted { get; private set; }
	public bool isComplete { get ; private set; }
    
    // Set the bools
	void Awake()
	{
		isStarted = false;	
		isComplete = false;
	}
	
	void Update()
	{
        // If the timer has started, update the time
		if(isStarted)
		{
			UpdateTime();
		}
        
    }

	//Sets timer variables
	public void SetTimer(int _min, int _sec)
	{
		minutes = _min;
		seconds = _sec;
		gameTimer = 1;
	}

	//Starts timer
	public void StartTimer()
	{
		isComplete = false;
		isStarted = true;
	}

	//Stops/pauses timer
	public void StopTimer()
	{
		isStarted = false;
	}

    // Debug line so specific variables for this timer can be seen in the inspector
	public void DebugString()
	{
		Debug.Log(name + " Minutes: " + minutes.ToString () + ", Seconds: " + seconds.ToString ());
	}
	
	// Function for updating the time
	public void UpdateTime()
	{
        if (!isComplete)
		{
			// Start the countdown for the gameTimer
			gameTimer -= Time.deltaTime;
            DebugString();
            // When it reaches 0
            if (gameTimer <= 0) 
			{
                // Remove a second
                seconds--;
				// Once the seconds are below 0
				if(seconds <= 0)
				{
					// Check if the minutes are above 0
					if(minutes > 0)
					{
						// Remove a minute
						minutes--;
						// Set the seconds to 59
						seconds = 59;
					}
				}
				// Set the gameTimer back to 1
				gameTimer = 1;
			}
		}
        //End timer
        if (minutes <= 0 && seconds <= 0)
		{
            // Stop the timer, set the complete timer bool to true
            StopTimer();
			isComplete = true;
		}
	}
}
