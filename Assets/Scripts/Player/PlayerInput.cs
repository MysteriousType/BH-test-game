namespace Assets.Scripts.Player
{
    using UnityEngine;

    public static class PlayerInput
    {
        private const string HorizontalAxis = "Horizontal";
        private const string VerticalAxis = "Vertical";

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
    }
}
