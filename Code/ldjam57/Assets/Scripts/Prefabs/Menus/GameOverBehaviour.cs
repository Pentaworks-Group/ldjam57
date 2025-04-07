using Assets.Scripts.Base;
using Assets.Scripts.Constants;
using Assets.Scripts.Scenes.Menues;
using System.Collections.Generic;
using UnityEngine;

public class GameOverBehaviour : BaseMenuBehaviour
{
    private void Start()
    {
        var outroAudio = new List<AudioClip>()
            {
                GameFrame.Base.Resources.Manager.Audio.Get("Game_over")
            };
        GameFrame.Base.Audio.Background.ReplaceClips(outroAudio);
    }

    public void GoToMainMenu()
    {
        Core.Game.PlayButtonSound();
        Core.Game.ChangeScene(SceneNames.MainMenu);
    }
}
