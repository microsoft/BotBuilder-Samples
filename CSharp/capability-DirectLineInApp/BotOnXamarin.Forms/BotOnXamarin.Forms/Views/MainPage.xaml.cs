using BotOnXamarin.Forms.Services;
using BotOnXamarin.Forms.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BotOnXamarin.Forms.Views
{
	public partial class MainPage : ContentPage
	{
        BotConnectorService service;

        public MainPage()
		{
			InitializeComponent();
            BindingContext = new MainViewModel();
        }
	}
}
