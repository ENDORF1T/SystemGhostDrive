using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Application.Game.UI
{
    public class UIButton_StartGame : MonoBehaviour
    {
        /// <summary>
        /// ��������� ������� �� ������
        /// </summary>
        public void OnClicked()
        {
            SceneManager.LoadScene("MainScene");
        }
    }
}