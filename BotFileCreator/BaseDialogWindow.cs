using Microsoft.VisualStudio.PlatformUI;

namespace BotFileCreator
{
    public class BaseDialogWindow : DialogWindow
    {
        public BaseDialogWindow()
        {
            this.HasMaximizeButton = true;
            this.HasMinimizeButton = true;
        }
    }
}
