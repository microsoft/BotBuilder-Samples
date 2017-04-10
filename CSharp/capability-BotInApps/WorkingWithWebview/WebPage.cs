namespace WorkingWithWebview
{
    using Xamarin.Forms;

    public class WebPage : ContentPage
    {
        public WebPage()
        {
            var browser = new WebView();

            browser.Source = "https://webchat.botframework.com/embed/<INCLUDE YOUR BOT WEBCHAT URL>";

            this.Content = browser;
        }
    }
}