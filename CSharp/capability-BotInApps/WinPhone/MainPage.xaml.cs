namespace WorkingWithWebview.WinPhone
{
    using Microsoft.Phone.Controls;

    public partial class MainPage : global::Xamarin.Forms.Platform.WinPhone.FormsApplicationPage 
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.SupportedOrientations = SupportedPageOrientation.PortraitOrLandscape;

            global::Xamarin.Forms.Forms.Init();

            this.LoadApplication(new WorkingWithWebview.App()); // new in 1.3
        }
    }
}
