using Assets.Scripts.Base;
using System.Collections.Generic;
using UnityEngine;
using GameFrame.Core.Extensions;

public class MusicBehauviour : MonoBehaviour
{
    private AudioClip transitionUp = GameFrame.Base.Resources.Manager.Audio.Get("Transition_Up");
    private AudioClip transitionDown = GameFrame.Base.Resources.Manager.Audio.Get("Transition_Down");

    private List<AudioClip> lowEnergyAudioClips = new List<AudioClip>()
    {
        GameFrame.Base.Resources.Manager.Audio.Get("Menu_empty"),
        GameFrame.Base.Resources.Manager.Audio.Get("Menu_empty"),
        GameFrame.Base.Resources.Manager.Audio.Get("Menu_empty"),
        GameFrame.Base.Resources.Manager.Audio.Get("Menu_1"),
        GameFrame.Base.Resources.Manager.Audio.Get("Menu_2")
    };

    private List<AudioClip> highEnergyAudioClips = new List<AudioClip>()
    {
        GameFrame.Base.Resources.Manager.Audio.Get("Game_high_empty"),
        GameFrame.Base.Resources.Manager.Audio.Get("Game_high_empty"),
    };

    private List<AudioClip> effectsClipList = new List<AudioClip>();

    //Music Energy Level
    private bool isHighEnergy = false;
    //Next high energy Music Time
    private float nextEnergySwitchTime = 0;

    //Random Ambient Sounds
    private float nextSoundEffectTime = 0;

    // Update is called once per frame
    void Update()
    {
        if (Core.Game.isRunning)
        {
            PlayRandomEffectSound();
        }
    }

    private void PlayRandomEffectSound()
    {
        if (Core.Game.State.TimeElapsed > nextSoundEffectTime && nextSoundEffectTime != 0)
        {
            GameFrame.Base.Audio.Effects.Play(effectsClipList.GetRandomEntry());
            double randomNumber = UnityEngine.Random.value;

            nextSoundEffectTime = (float)(randomNumber * 20.0 + 5.0 + Core.Game.State.TimeElapsed);
        }
        else if (nextSoundEffectTime == 0)
        {
            double randomNumber = UnityEngine.Random.value;

            nextSoundEffectTime = (float)(randomNumber * 20.0 + 5.0 + Core.Game.State.TimeElapsed);
        }
    }

    private void changeMusicEnergyUp()
    {
        GameFrame.Base.Audio.Background.PlayTransition(transitionUp, highEnergyAudioClips);
    }

    private void changeMusicEnergyDown()
    {
        GameFrame.Base.Audio.Background.PlayTransition(transitionDown, lowEnergyAudioClips);
    }

}
