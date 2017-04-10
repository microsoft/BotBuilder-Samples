#region Generated Code
namespace WorkingWithWebview.iOS
{
    using Foundation;
    using UIKit;

    [Register("AppDelegate")]
    public partial class AppDelegate :
    global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate // superclass new in 1.3
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            this.LoadApplication(new App());  // method is new in 1.3

            return base.FinishedLaunching(app, options);
        }
    }
}
#endregion