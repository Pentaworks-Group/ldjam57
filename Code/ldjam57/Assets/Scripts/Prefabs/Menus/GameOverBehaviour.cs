using Assets.Scripts.Base;
using Assets.Scripts.Constants;
using Assets.Scripts.Scenes.Menues;
using System.Collections.Generic;
using UnityEngine;

public class GameOverBehaviour : BaseMenuBehaviour
{
    private void Start()
    {
        var outroAudio = GameFrame.Base.Resources.Manager.Audio.Get("Game_over");
        var menuAudio = new List<AudioClip>()
            {
                GameFrame.Base.Resources.Manager.Audio.Get("Percussion")
            };
        GameFrame.Base.Audio.Background.PlayTransition(outroAudio, menuAudio);
    }

    public void GoToMainMenu()
    {
        Core.Game.PlayButtonSound();
        Core.Game.ChangeScene(SceneNames.MainMenu);
    }
}
