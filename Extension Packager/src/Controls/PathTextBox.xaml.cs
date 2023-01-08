// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Extension_Packager_Library.src.Helper;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Extension_Packager.src.Controls
{
    public sealed partial class PathTextBox : UserControl
    {
        public enum PickerTypes
        {
            Folder,
            File
        }

        public string Label { get; set; } = "*Set Label*";
        public string PlaceholderText { get; set; } = "*Set Placeholder*";
        public string ButtonContent { get; set; } = "Durchsuchen";
        public string PickerFileTypes { get; set; } = "*";
        public PickerTypes PickerType { get; set; } = PickerTypes.File;
        public bool IsReadOnly { get; set; } = true;


        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
                                 DependencyProperty.Register("Value", typeof(string),
                                                             typeof(PathTextBox), 
                                                             PropertyMetadata.Create("blblbl"));

        public MyCommand OpenFilesDialogCommand { get; set; }

        public PathTextBox()
        {
            InitializeComponent();
            OpenFilesDialogCommand = new MyCommand(OpenFilesDialog);
        }

        private async void OpenFilesDialog(object parameter = null)
        {
            IPicker picker = PickerType == PickerTypes.File ? new FilePicker() : new FolderPicker();
            Value = await picker.GetPathDialog(PickerFileTypes);
        }
    }
}
