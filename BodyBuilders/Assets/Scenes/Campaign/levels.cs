/*
Things to keep in mind while designing levels:

    - Make sure that the player can't get through an area by ejecting if the challenge is supposed to be more difficult, or if they require the part later on.
    - These levels are set up primarily to teach new mechanics in an order that I think makes sense. Obviously, we would need to test to see if players understand as they go.
    - You can add levels between these, but make sure not to include mechanics that have not been introduced yet.

Level 1:

    - Introduce attach with legs (needed for higher jumping)
    - Basic platforming
    - Introduce switch doors
    - Introduce detach with legs (needed for getting through a tight space) to press a button and open another door
    - Introduce ejecting for extra jump height

Level 2:

    - Introduce arms
    - Introduce climbing
    - Introduce boxes and picking up boxes
        - This will be used initially as a platform to get to a higher level (no eject required)
        - The second will be used with the introduction of pressure pads

Level 3:

    - Platforming with head, then arms, then gaining legs
    - Introduction to gyres
        - Need to bounce on a gyre head to get over a ledge
    - Puzzle:
        - The player must cross a chasm, open a door on the other side, and then climb a ladder to the level's end
        - The pressure button used to open the door is at the bottom of the chasm, and there is a path leading back up along the left (using pass-through platforms)
        - The chasm is too large to jump over without ejecting, so the player must just with a full-body, and eject their legs in mid-air, causing the legs to fall onto the pressure button, and opening the door
        - The player can then climb the ladder at the end using their arms

Level 4:
Level 4 will largely be about introducing kinetic map features A lot of challenges should involve seeing several split pathways throughout the level. Each time, the shorter path ends with a button, necessary for opening up a door or activating a fan or elevator to continue progressing.

    - Platforming with head and legs
    - Gain the afterburner legs
    - Introduction to elevators and fans
        - Using fans for sinusoidal movement (landing at the bottom of each fan area so you can get the maximum amount of boost added to your vertical momentum)
    - Introduction to activatable elevators and fans using buttons
    - Introduction to conveyor belts

Level 5:

    - Platforming with full body
    - Need to carry boxes around for pressure pads to unlock fans, doors and elevators necessary for progressing
    - Introduction to bladebots throughout jump puzzle
    - Puzzle:
        - A button at the bottom of a ledge, at the end of a conveyor belt, operates two doors
        - The doors are positioned in a hallway above the conveyor belt, and they bar the player from the end of the level
        - The button is switch button, and each door is out of sync, so pressing the button opens the furthermost door, and closes the closest door
        - To get through, the player will need to drop a box on the conveyor belt, and move through the first door, then move through the second door before the box hits the pressure pad

Level 6:

    - Introduction to timed buttons
    - Bladebot corridor puzzles (with the time limit of timed buttons)
    - Introduce that bladebots destroy destructible terrain
    - Introduce groundbreaker legs, and show that they can also destroy terrain
    - Introduce banshees
    - Banshee platforming puzzle
        - Put platforms and walls between yourself and the banshee before it fires the death laser

Level 7:

    - Introduce powerlifter arms and energy cells
        - Position energy cells so that they block the laser from hitting you, and get charged
        - Put charged power cells in power stations to activate linked objects (in this case, a door)
    - Force the player to discard their arms and swap out for shield arms
    - The level at this stage will consist of a linear group of rectangular rooms.
    - In the first room, the player will be unable to pass through a closed door.
        - There is a banshee in the middle of the room.
        - There is also a pair of laser routers in the room; one is positioned in the far wall above the door, and is connected to the door by a wire. It is pointed to the right. The other laser router is positioned at the top left of the room and is pointed right at the one above the door.
        - The player needs to dodge the laser and cause it to fire at the router, triggering it and causing it to direct the laser into the one above the door.
        - This activates the door and causes the laser router above it to continue the laser through into the next room.
        - Do a few more puzzles which involve directing the laser to open the door

Level 8:

    - Introduce the deflector shield and laser networks
        - The player must redirect a banshee laser to a laser router, and then position themselves to use that laser to hit the next, and so on

_______________________________________________________________________________________________________________________________________________________________________________________________________

From here on, we can start using lasers as both help and hindrance. The player must redirect the laser to open pathways, but we can then use laser routing systems as laser obstacle courses
_______________________________________________________________________________________________________________________________________________________________________________________________________

Level 9:

    - - The player redirects a laser to a laser router above a door (like in level 7), letting it continue through to the next room
    - The player must then disconnect from the deflector shield arms to get through a small space
        - The player must now roll through a level, jumping between platforms, as laser routers that they charged patrol back and forth. Every time they pass a gap between platforms, the laser will shoot through, killing the player if they get caught in the beam
    - Give them back booster legs and let them continue through a difficult puzzle
        - Try to include an area where there are small platforms, with a continuous laser above and below
        - The player can't use the double jump quickly in this area without frying themselves on the top laser, so it will be about timing their jumps to get the most horizontal distance between platforms as possible

Level 10:

    - Introduce spider climb
        - Spider climb level as a template
        - Include arms and some box/laser problems in the middle
        - Bring back the laser grid jump puzzle towards the end, encouraging the player to ditch the arms for the mobility of the scaler augment head.

Level 11:

    - Introduce hookshot augment for head
        - Simple hookshot platforming with bladebots and banshees occasionally
    - Hookshot augment with scaler augment
        - Kinetic puzzles where the player must optimise movement by timing each swing, fan and elevator perfectly

Level 12:

    - A brief revision of all skills learned so far
    - The player must then get through a few areas quickly, as the Bladebot Edgelord chases them down straight corridors (with spike pit cliffs on either side so that the boss flies off-screen rather than into a wall and taking damage).
    - As the player ascends through different horizontal floors, chased by the boss on each, they will be required to put their skills to the test, opening up pathways to stay ahead of the boss or ascend to the next floor and get out of its path.
    - The boss will disappear after a few encounters, giving the player a bit of breathing room.
    - The boss will then return in an S-Bend section of the level, where it can now be damaged by colliding with the solid bounding walls of the level.
    - After escaping the boss and letting it destroy itself, the player leaves through the top of the underground facility, escaping through the ground floor door to the world above.
*/