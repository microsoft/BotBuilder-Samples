namespace ComponentDialogs
{
    /// <summary>
    /// Contains the user profile information.
    /// </summary>
    public class UserProfile
    {
        public CheckInDialog.GuestInfo Guest { get; set; }

        public ReserveTableDialog.TableInfo Table { get; set; }

        public WakeUpCallDialog.WakeUpInfo WakeUp { get; set; }
    }
}
