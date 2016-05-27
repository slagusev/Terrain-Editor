﻿using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.SimpleChildWindow;

namespace TerrainEditor.UserControls
{

    public partial class UvMappingPropertyEditor : UserControl
    {
        private SelectMappingDialog m_mappingDialog;

        public UvMappingPropertyEditor()
        {
            InitializeComponent();
        }
        private void SelectNewMapping(object sender, RoutedEventArgs e)
        {
            m_mappingDialog = new SelectMappingDialog();
            m_mappingDialog.ClosingFinished += MappingDialogOnClosingFinished;

            Application.Current.MainWindow.ShowChildWindowAsync(m_mappingDialog);
        }
        private void MappingDialogOnClosingFinished(object sender, RoutedEventArgs routedEventArgs)
        {
            SetValue(DataContextProperty, m_mappingDialog.SelectedMapping ?? DataContext);

            m_mappingDialog.ClosingFinished -= MappingDialogOnClosingFinished;
            m_mappingDialog = null;
        }
    }
}
