using System;

namespace PAARC.WP7.Controls
{
    public class TextInputEventArgs : EventArgs
    {
        /// <summary>
        /// The text that is the result of the input, or <c>null</c> if there was no result.
        /// </summary>
        public string Text
        {
            get;
            private set;
        }

        public TextInputEventArgs(string text)
        {
            Text = text;
        }
    }
}