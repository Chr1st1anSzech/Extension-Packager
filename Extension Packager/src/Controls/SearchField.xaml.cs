// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager_Library.src.Helper;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using System.Windows.Input;

namespace Extension_Packager.src.Controls
{
    public sealed partial class SearchField : UserControl
    {
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
                                 DependencyProperty.Register("Command", typeof(ICommand),
                                                             typeof(SearchField), new PropertyMetadata(null));

        public SearchField()
        {
            this.InitializeComponent();
        }

        private void SearchFieldTextBox_KeyUp(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key != Windows.System.VirtualKey.Enter) return;

            Command.Execute(SearchFieldTextBox.Text);
        }

        private void SearchFieldTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchFieldTextBox.Text.Length == 0)
            {
                Command.Execute(string.Empty);
            }
        }
    }
}
