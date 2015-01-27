using System;
using Coding4Fun.Phone.Controls;

namespace PAARC.WP7.Controls
{
    public class TextInputHelper
    {
        private InputPrompt _prompt;

        public event EventHandler<TextInputEventArgs> TextInputFinished;

        public bool IsActive
        {
            get;
            set;
        }

        public void ShowTextInputPrompt(string defaultValue)
        {
            IsActive = true;

            // show input prompt from the coding4fun toolkit
            _prompt = new InputPrompt();
            _prompt.IsSubmitOnEnterKey = false;
            _prompt.IsCancelVisible = true;
            _prompt.Message = "Enter the text you want to send:";
            _prompt.Title = string.Empty;
            _prompt.Value = defaultValue;
            _prompt.Completed += InputPrompt_Completed;
            _prompt.Show();
        }

        private void InputPrompt_Completed(object sender, PopUpEventArgs<string, PopUpResult> popUpEventArgs)
        {
            _prompt = null;
            IsActive = false;

            if (popUpEventArgs.PopUpResult != PopUpResult.Ok || string.IsNullOrEmpty(popUpEventArgs.Result))
            {
                // no result
                RaiseTextInputFinishedEvent(null);
                return;
            }

            // raise event with text
            var text = popUpEventArgs.Result;
            RaiseTextInputFinishedEvent(text);
        }

        private void RaiseTextInputFinishedEvent(string text)
        {
            var handlers = TextInputFinished;
            if (handlers != null)
            {
                handlers(this, new TextInputEventArgs(text));
            }
        }
    }
}
