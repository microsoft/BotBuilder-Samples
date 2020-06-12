// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using System.Windows.Media;
using System.Collections.Generic;
using System;

namespace BotManagerVSIX
{
    /// <summary>
    /// Interaction logic for BotManagerMainView.xaml
    /// </summary>
    public partial class BotManagerMainView : Window
    {
        public BotManagerMainView()
        {
            InitializeComponent();
            CreateButton.Visibility = Visibility.Collapsed;
            Application.Current.MainWindow = this;
            MenuQnAService.Visibility = Visibility.Collapsed;
            MenuQnAKB.Visibility = Visibility.Collapsed;
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            TabItem currentTabItem = (TabItem)ControlMenuItems.SelectedItem;

            TabItem nextTabItem = FindNextVisibleTabitem(currentTabItem);

            ControlMenuItems.SelectedItem = nextTabItem;

            if (nextTabItem.Name == TabChannelRegistration.Name)
            {
                NextButton.Visibility = Visibility.Collapsed;
                CreateButton.Visibility = Visibility.Visible;
            }
            else
            {
                CreateButton.Visibility = Visibility.Collapsed;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            TabItem currentTabItem = (TabItem)ControlMenuItems.SelectedItem;

            if (currentTabItem.Name == TabAzure.Name)
            {
                return;
            }

            TabItem nextTabItem = FindNextVisibleTabitem(currentTabItem, true);

            ControlMenuItems.SelectedItem = nextTabItem;

            NextButton.Visibility = Visibility.Visible;
            CreateButton.Visibility = Visibility.Collapsed;
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            ControlMenuItems.SelectedIndex += 1;
            NextButton.Visibility = Visibility.Collapsed;
            BackButton.Visibility = Visibility.Collapsed;
            CreateButton.Visibility = Visibility.Collapsed;
            CancelButton.Content = "Close";

        }

        private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void ControlMenuItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                TabControl tabControl = (TabControl)sender;
                TabItem tabItem = (TabItem)tabControl.SelectedItem;
                List<MenuItem> items = menuItems.Items
                    .Cast<MenuItem>()
                    .ToList();

                items
                    .ForEach(x => x.Background = Brushes.Transparent);

                MenuItem menuItem = items
                    .Where(item => item.Tag.ToString() == tabItem.Name)
                    .FirstOrDefault();

                var bc = new BrushConverter();

                menuItem.Background = (Brush)bc.ConvertFrom("#FFD9E0F8");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private TabItem FindNextVisibleTabitem(TabItem currentTabItem, bool reverse = false)
        {
            try
            {
                List<MenuItem> items = menuItems.Items
                    .Cast<MenuItem>()
                    .ToList();

                if (reverse)
                {
                    items.Reverse();
                }

                int nextIndex = items.FindIndex(item => item.Tag.ToString() == currentTabItem.Name) + 1;

                MenuItem nextMenu = items
                    .Skip(nextIndex)
                    .Where(item => item.Visibility == Visibility.Visible)
                    .FirstOrDefault();

                TabItem nextTab = (TabItem)ControlMenuItems.FindName(nextMenu.Tag.ToString());

                return nextTab;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
