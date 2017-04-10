namespace WorkingWithWebview.Android
{
    using global::Android.App;
    using global::Android.Content.PM;
    using global::Android.OS;

    [Activity(Label = "WorkingWithWebview.Android.Android", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            this.LoadApplication(new App());
        }
    }
}