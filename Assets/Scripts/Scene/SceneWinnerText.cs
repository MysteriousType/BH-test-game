namespace Assets.Scripts.Scene
{
    using Mirror;
    using UnityEngine;
    using UnityEngine.UI;

    public class SceneWinnerText : NetworkBehaviour
    {
        [Header("UI")]
        [SerializeField]
        private Text _winnerCanvasText;

        [SyncVar(hook = nameof(OnWinnerTextChanged))]
        private string _text = string.Empty;

        public void ResetText() => _text = string.Empty;

        public void SetText(string text)
        {
            if (_text == string.Empty)
            {
                _text = text;
            }
        }

        private void OnWinnerTextChanged(string oldText, string newText)
        {
            _winnerCanvasText.text = _text;
        }
    }
}
