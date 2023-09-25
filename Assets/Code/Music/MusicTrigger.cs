using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    public void PlayMainMenuTheme() {
        MusicManager.Instance.PlayMainMenuTheme();
    }

    public void PlayWorldTheme() {
        MusicManager.Instance.PlayWorldTheme();
    }

}
