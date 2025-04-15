using Microsoft.Extensions.Logging;
using RemindMe.Services;

namespace RemindMe;

public partial class AssistedRoom : ContentPage
{
	public AssistedRoom()
	{
		InitializeComponent();
	}

    private async void WatchLatestVideo(object sender, EventArgs e)
    {
        var client = new BackendClient(new Logger<BackendClient>(new LoggerFactory()));
        var response = await client.GetLatestVideo().ConfigureAwait(false);

        if (response.Success)
        {
            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(response.Data)
            });
        }
        else
        {
            await DisplayAlert("Error", response.Message, "OK");
        }
    }
}