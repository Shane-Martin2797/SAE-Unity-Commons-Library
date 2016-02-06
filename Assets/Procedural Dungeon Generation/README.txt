1. Make an Empty of where you wish for the dungeon to start spawning.
2. Create prefabs and attach the GenerationSection script to them.
3. Fill out the details in inspector on these Sections.

The Section Used in this example is the “4-Way Intersection”
E.g. >All Entries
	Size: 4 //This is the amount of doors the prefab has (Entry points)
	
	> Element 0
		Location: UP //This is which side the door is located on
		Offset: X: 0 Y: 0 Z: 0 	//This is the offset from the centre
					//On the side of the door. Since it
					//is up, it measures from the upper
					//bounds and then adds the offset.
		Available: [true] 	//This should be ticked if you wish
					//for something to spawn on here.
	> Element 1
		Location: Left
		Offset: (0,0,0)
		Available: [true]
	> Element 2
		Location: Down
		Offset: (0,0,0)
		Available: [true]
	> Element 3
		Location: Right
		Offset: (0,0,0)
		Available: [true]

4. Add this prefab to the list called “Section Prefabs” and give it a ratio
to increase/decrease the spawn rate.

5. Assign a target for the objects to follow, so they can continue generating.

6. Set a soft edge so that the sections will generate further than the player 
can see. It is the number under the info box that says:
“Distance between prefab and object before it disables”

7. Click Play to Start Generating.

8. (Optional) Check the box Minimise Consecutive Repeats and then set a
number of maximum consecutive repeats of the same section generating.

9. (Optional) Change the method CodeToDoWhenEncounteringAPlaceWhereNothingCanSpawn
to do something that you want it to do.