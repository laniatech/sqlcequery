﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ChristianHelle.DatabaseTools.SqlCe.QueryAnalyzer.View;
using ChristianHelle.DatabaseTools.SqlCe.QueryAnalyzer.ViewModel;

namespace ChristianHelle.DatabaseTools.SqlCe.QueryAnalyzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool dirty;

        public MainWindow()
        {
            InitializeComponent();
        }

        private MainViewModel ViewModel
        {
            get { return ((MainViewModel)DataContext); }
        }

        static void SafeOperation(Action annonymousMethod)
        {
            try
            {
                annonymousMethod.Invoke();
            }
            catch (Exception e)
            {
#if DEBUG
                MessageBox.Show(e.ToString(), "Unexpected Error");
#else
                MessageBox.Show(e.Message, "Unexpected Error");
#endif
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SafeOperation(() =>
            {
                if (!ViewModel.LaunchedWithArgument)
                    ViewModel.OpenDatabase();
                else
                    ViewModel.ProcessCommandLineArguments();
            });
        }

        private void ExecuteQuery_Click(object sender, RoutedEventArgs e)
        {
            SafeOperation(() => ViewModel.ExecuteQuery(editor.SelectedText));
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            SafeOperation(() =>
            {
                switch (e.Key)
                {
                    case Key.F5:
                        ViewModel.ExecuteQuery(editor.SelectedText);
                        break;
                    case Key.F1:
                        new AboutBox(this).ShowDialog();
                        break;
                }
            });
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            new AboutBox(this).ShowDialog();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            SafeOperation(() => ViewModel.OpenDatabase());
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            dirty = true;
        }

        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            SafeOperation(() =>
            {
                if (!dirty) return;
                ViewModel.SaveTableDataChanges();
                dirty = false;
            });
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            SafeOperation(() => ViewModel.LoadDroppedFile(e.Data));
        }

        private void Shrink_Click(object sender, RoutedEventArgs e)
        {
            SafeOperation(() => ViewModel.ShrinkDatabase());
        }

        private void Compact_Click(object sender, RoutedEventArgs e)
        {
            SafeOperation(() => ViewModel.CompactDatabase());
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}