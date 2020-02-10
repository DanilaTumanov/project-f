using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Controllers
{
    public class MainMenuController : MonoBehaviour
    {
        [Serializable]
        private struct ButtonInfo
        {
            public Button button;
            public string sceneName;
        }
        
        [SerializeField] private ButtonInfo[] _menuButtons;

        private void Awake()
        {
            foreach (var buttonInfo in _menuButtons)
            {
                buttonInfo.button
                    .onClick
                    .AddListener(() => SceneManager.LoadScene(buttonInfo.sceneName));
            }
        }
    }
}