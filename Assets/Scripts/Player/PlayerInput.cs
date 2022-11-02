namespace Assets.Scripts.Player
{
    using UnityEngine;

    public static class PlayerInput
    {
        private const string HorizontalAxis = "Horizontal";
        private const string VerticalAxis = "Vertical";
        private const string MouseX = "Mouse X";
        private const string MouseY = "Mouse Y";
        private const int LeftMouseButtonIndex = 1;

        public static Vector2 MouseAxis
        {
            get
            {
                float x = Input.GetAxis(MouseX);
                float y = Input.GetAxis(MouseY);
                return new Vector2(x, y);
            }
        }

        /// <summary>
        /// Axis raw input, where X is a horizontal axis and Y is a vertical one.
        /// </summary>
        public static Vector2 AxisNormalized
        {
            get
            {
                float horizontal = Input.GetAxis(HorizontalAxis);
                float vertical = Input.GetAxis(VerticalAxis);
                return new Vector2(horizontal, vertical).normalized;
            }
        }

        public static bool Dash => Input.GetMouseButton(LeftMouseButtonIndex);
    }
}
