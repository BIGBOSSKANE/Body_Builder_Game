/*
Creator: Daniel
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class songSwitcher : MonoBehaviour
{
    [Range(1 , 4)] int partConfiguration;
    public enum Intensity {Calm , Tension};
    public Intensity intensity;
    public enum Gameplay {Boss , Exploration};
    public Gameplay gameplay;
    public bool Puzzle;
    public bool stopMusic;

    string intensityString;
    string gameplayString;
    string puzzleString;


    void Start()
    {
        if(intensity == Intensity.Calm)
        {
            intensityString = "Calm";
        }
        else if(intensity == Intensity.Tension)
        {
            intensityString = "Tension";
        }

        if(gameplay == Gameplay.Boss)
        {
            gameplayString = "Boss";
        }
        else if(gameplay == Gameplay.Exploration)
        {
            gameplayString = "Exploration";
        }

        if(Puzzle) puzzleString = "Puzzle";
        else puzzleString = "None";
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            if(stopMusic)
            {
                AkSoundEngine.SetState("Mode" , "None");
                return;
            }
            
            AkSoundEngine.SetState("Puzzle" , puzzleString);
            AkSoundEngine.SetState("Intensity" , intensityString);
            AkSoundEngine.SetState("Alive" , "Alive");
            AkSoundEngine.SetState("Mode" , "Gameplay");
            AkSoundEngine.SetState("Gameplay" , gameplayString);
        }
    }
}
