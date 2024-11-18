using TMPro;
using UnityEngine;

namespace Project.Application.Game.Lap.UI
{
    public class UI_LapInformation : MonoBehaviour
    {
        [SerializeField] private TMP_Text _textField = null;

        private void RefreshUI()
        {
            if (!LapSystem.Instance) return; 
            _textField.text = $"{LapSystem.Instance.CurrentLap}/{LapSystem.Instance.MaxLap}"; // вывод данных в формате "0/0"
        }

        private void OnDisable()
        {
            LapSystem.OnChangeProperty -= RefreshUI; 
        }

        private void Start()
        {
            LapSystem.OnChangeProperty += RefreshUI;

            RefreshUI();
        }
    }
}