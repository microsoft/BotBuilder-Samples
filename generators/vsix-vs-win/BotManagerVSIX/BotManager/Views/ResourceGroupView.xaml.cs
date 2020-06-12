// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BotManagerVSIX
{
    /// <summary>
    /// Interaction logic for ResourceGroupView.xaml
    /// </summary>
    public partial class ResourceGroupView : Window
    {
        public ResourceGroupView()
        {
            InitializeComponent();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NameText.Text = "";
            Close();
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public static bool OpenAndSyncResourceGroupView(ComboBox resourceGroupCombo)
        {
            try
            {
                ResourceGroupView w = new ResourceGroupView();
                w.Owner = Application.Current.MainWindow;

                if (w.ShowDialog() == false)
                {
                    string rg = w.NameText.Text;

                    if (rg.Length == 0) return false;

                    int i = resourceGroupCombo.Items.IndexOf(rg);

                    if (i == -1)
                    {
                        i = resourceGroupCombo.Items.Add(rg);
                    }

                    resourceGroupCombo.SelectedIndex = i;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        private void NameText_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                Confirm.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }
    }
}
