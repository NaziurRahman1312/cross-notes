using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace CrossNote.ViewModels
{
    internal class AboutViewModel
    {
        #region Fields

        public string Title => AppInfo.Name;
        public string Version => AppInfo.VersionString;
        public string MoreInfoUrl => "https://aka.ms/maui";
        public string Message => "This app is written in XAML and C# with .NET MAUI.";

        #endregion

        #region Properties

        public ICommand ShowMoreInfoCommand { get; }

        #endregion

        #region Constructors

        public AboutViewModel()
        {
            ShowMoreInfoCommand = new AsyncRelayCommand(ShowMoreInfo);
        }

        #endregion

        #region Methods

        async Task ShowMoreInfo() =>
            await Launcher.Default.OpenAsync(MoreInfoUrl);

        #endregion
    }
}
