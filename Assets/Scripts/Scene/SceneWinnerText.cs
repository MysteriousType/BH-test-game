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

        [SyncVar(hook = nameof(OnStatusTextChanged))]
        private string _text;

        public void SetText(string text) => _text = text;

        private void OnStatusTextChanged(string oldText, string newText)
        {
            _winnerCanvasText.text = _text;
        }
    }
}
