namespace WorkingWithWebview
{
    using Xamarin.Forms;

    public class App : Application // superclass new in 1.3
    {
        public App()
        {
            this.MainPage = new WebPage { Title = "Web Page" };
        }
    }
}