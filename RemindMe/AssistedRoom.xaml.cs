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
        var client = new BackendClient();
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

    private async void SwapRole(object sender, EventArgs e)
    {
        if (await DisplayAlert("Swap Role", "Are you sure you want to swap roles? You will record videos instead of receiving them.", "Yes", "No").ConfigureAwait(false) == false)
        {
            return;
        }

        var client = new BackendClient();

        var response = await client.SwapRole().ConfigureAwait(false);
        if (response.Success)
        {
            await Shell.Current.GoToAsync($"//{nameof(ScheduleVideoPage)}").ConfigureAwait(false);
        }
    }
}