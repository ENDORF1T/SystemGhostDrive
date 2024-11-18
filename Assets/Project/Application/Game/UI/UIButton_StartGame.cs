using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Application.Game.UI
{
    public class UIButton_StartGame : MonoBehaviour
    {
        /// <summary>
        /// Обработка нажатия по кнопке
        /// </summary>
        public void OnClicked()
        {
            SceneManager.LoadScene("MainScene");
        }
    }
}