using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace AR.Wpf.TextBoxBehaviors
{
    public class NumericInputBehavior : Behavior<TextBox>
    {
        private const NumberStyles ValidNumberStyles = NumberStyles.Float;

        public NumericInputBehavior()
        {
            this.OnlyPositiveInput = false;
            this.OnlyIntegerInput = false;
        }

        public static readonly DependencyProperty OnlyPositiveInputProperty =
         DependencyProperty.Register("OnlyPositiveInput", typeof(bool),
         typeof(NumericInputBehavior), new FrameworkPropertyMetadata(false));

        public bool OnlyPositiveInput
        {
            get => (bool)GetValue(OnlyPositiveInputProperty);
            set => SetValue(OnlyPositiveInputProperty, value);
        }

        public static readonly DependencyProperty OnlyIntegerInputProperty =
            DependencyProperty.Register("OnlyIntegerInput", typeof(bool),
                typeof(NumericInputBehavior), new FrameworkPropertyMetadata(false));

        public bool OnlyIntegerInput
        {
            get => (bool)GetValue(OnlyIntegerInputProperty);
            set => SetValue(OnlyIntegerInputProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewTextInput += AssociatedObjectPreviewTextInput;
            AssociatedObject.PreviewKeyDown += AssociatedObjectPreviewKeyDown;

            DataObject.AddPastingHandler(AssociatedObject, Pasting);

        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewTextInput -= AssociatedObjectPreviewTextInput;
            AssociatedObject.PreviewKeyDown -= AssociatedObjectPreviewKeyDown;

            DataObject.RemovePastingHandler(AssociatedObject, Pasting);
        }

        private void Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                var pastedText = (string)e.DataObject.GetData(typeof(string));

                if (!this.IsValidInput(this.GetText(pastedText)))
                {
                    System.Media.SystemSounds.Beep.Play();
                    e.CancelCommand();
                }
            }
            else
            {
                System.Media.SystemSounds.Beep.Play();
                e.CancelCommand();
            }
        }

        private void AssociatedObjectPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                if (!this.IsValidInput(this.GetText(" ")))
                {
                    System.Media.SystemSounds.Beep.Play();
                    e.Handled = true;
                }
            }
        }

        private void AssociatedObjectPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!this.IsValidInput(this.GetText(e.Text)))
            {
                System.Media.SystemSounds.Beep.Play();
                e.Handled = true;
            }

        }

        private string GetText(string input)
        {
            var txt = this.AssociatedObject;

            int selectionStart = txt.SelectionStart;
            if (txt.Text.Length < selectionStart)
                selectionStart = txt.Text.Length;

            int selectionLength = txt.SelectionLength;
            if (txt.Text.Length < selectionStart + selectionLength)
                selectionLength = txt.Text.Length - selectionStart;

            var realtext = txt.Text.Remove(selectionStart, selectionLength);

            int caretIndex = txt.CaretIndex;
            if (realtext.Length < caretIndex)
                caretIndex = realtext.Length;

            var newtext = realtext.Insert(caretIndex, input);

            return newtext;
        }

        private bool ValidateMinus(string input)
        {
            if (input.Contains("-"))
            {
                if (OnlyPositiveInput)
                    return false;

                if (input.IndexOf("-", StringComparison.Ordinal) > 0)
                    return false;

                if (input.Count(x => x == '-') > 1)
                    return false;

                if (input.Length == 1)
                    return true;
            }

            return true;
        }

        private bool ValidateValue(string input)
        {
            if (OnlyIntegerInput && OnlyPositiveInput)
                return uint.TryParse(input, ValidNumberStyles, CultureInfo.CurrentCulture, out var u);
            if (OnlyIntegerInput)
                return int.TryParse(input, ValidNumberStyles, CultureInfo.CurrentCulture, out var i);

            if (!OnlyPositiveInput && (input == "-"))
                return true;

            return decimal.TryParse(input, ValidNumberStyles, CultureInfo.CurrentCulture, out var d) ||
                   decimal.TryParse(input, ValidNumberStyles, CultureInfo.InvariantCulture, out d);
        }

        private bool IsValidInput(string input)
        {
            if (OnlyIntegerInput)
            {
                return input.All(c => "0123456789-".Contains(c)) && ValidateMinus(input) && ValidateValue(input);
            }

            if (input.Count(x => x == ',' || x == '.') > 1)
                return false;

            if (!ValidateMinus(input))
                return false;

            return ValidateValue(input);
        }
    }
}