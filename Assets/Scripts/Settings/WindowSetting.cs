using System.Collections;
using UnityEngine;

namespace Gameplay
{
    public class WindowSetting : MonoBehaviour
    {
        public void SwitchWindowMode(int windowType)
        {
            //Self explanatory
            switch (windowType)
            {
                case 0:
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    break;
                case 1:
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                    break;
            }
        }
    }
}
