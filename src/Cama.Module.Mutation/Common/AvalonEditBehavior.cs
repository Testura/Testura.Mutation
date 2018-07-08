using System;
using System.Windows;
using System.Windows.Interactivity;
using ICSharpCode.AvalonEdit;

namespace Cama.Module.Mutation.Common
{
    public class AvalonEditBehavior : Behavior<TextEditor>
    {
        public static readonly DependencyProperty GiveMeTheTextProperty =
            DependencyProperty.Register(
                "CodeText",
                typeof(string),
                typeof(AvalonEditBehavior),
                new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PropertyChangedCallback));

        public string CodeText
        {
            get { return (string)GetValue(GiveMeTheTextProperty); }
            set { SetValue(GiveMeTheTextProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject != null)
            {
                AssociatedObject.TextChanged += AssociatedObjectOnTextChanged;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (AssociatedObject != null)
            {
                AssociatedObject.TextChanged -= AssociatedObjectOnTextChanged;
            }
        }

        private static void PropertyChangedCallback(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var behavior = dependencyObject as AvalonEditBehavior;
            var editor = behavior.AssociatedObject;
            if (editor?.Document != null)
            {
                var caretOffset = editor.CaretOffset;
                editor.Document.Text = dependencyPropertyChangedEventArgs.NewValue.ToString();
                editor.CaretOffset = caretOffset;
            }
        }

        private void AssociatedObjectOnTextChanged(object sender, EventArgs eventArgs)
        {
            var textEditor = sender as TextEditor;
            if (textEditor?.Document != null)
            {
                CodeText = textEditor.Document.Text;
            }
        }
    }
}
