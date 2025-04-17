using Microsoft.Extensions.Logging;
using RemindMe.Services;

namespace RemindMe;

public partial class ScheduleVideoPage : ContentPage
{
    public FileResult? submittingVideo;

    public ScheduleVideoPage()
    {
        InitializeComponent();
    }

    private async void ScheduleOrRecordVideo(object sender, EventArgs e)
    {
        await GenerateVideo();
    }

    private async void ViewRecordingTips(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"//{nameof(TipsPage)}").ConfigureAwait(false);
    }

    private async void SwapRole(object sender, EventArgs e)
    {
        if (await DisplayAlert("Swap Role", "Are you sure you want to swap roles? You will receive videos instead of sending them.", "Yes", "No").ConfigureAwait(false) == false)
        {
            return;
        }

        var client = new BackendClient(new Logger<BackendClient>(new LoggerFactory()));

        var response = await client.SwapRole().ConfigureAwait(false);
        if (response.Success)
        {
            await Shell.Current.GoToAsync($"//{nameof(AssistedRoom)}").ConfigureAwait(false);
        }
    }

    private async void PublishSelectedVideo(object sender, EventArgs e)
    {
        if (submittingVideo == null)
        {
            await DisplayAlert("Select a video", "You have not selected or recorded a video to be uploaded.", "OK");
            return;
        }

        var result = await DisplayAlert("Publish Video", "Are you sure you want to schedule this video?", "Yes", "No");
        if (result)
        {
            var client = new BackendClient(new Logger<BackendClient>(new LoggerFactory()));
            var response = await client.PublishVideo(submittingVideo, DatePicker.Date, TimePicker.Time);

            if (response.Success)
            {
                await DisplayAlert("Success", "Your video has been successfully scheduled.", "OK");
            }
            else
            {
                await DisplayAlert("Error", response.Message, "OK");

            }
        }
    }

    private async Task GenerateVideo()
    {
        string action = await DisplayActionSheet("Add a Video", "Cancel", null, "Record Video", "Upload from Device");

        switch (action)
        {
            case "Record Video":
                await RecordVideo();
                break;

            case "Upload from Device":
                await SelectVideoFromGallery();
                break;

            default:
                break;
        }
    }

    async Task RecordVideo()
    {
        try
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                var video = await MediaPicker.Default.CaptureVideoAsync();

                if (video != null)
                {
                    await SetOrRplaceVideo(video);
                }
            }
            else
            {
                await DisplayAlert("Not Supported", "Video recording is not supported on this device.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    async Task SelectVideoFromGallery()
    {
        var result = await FilePicker.Default.PickAsync(new PickOptions
        {
            FileTypes = FilePickerFileType.Videos,
            PickerTitle = "Select a video"
        });

        if (result != null)
        {
            await SetOrRplaceVideo(result);
        }
    }

    private async Task SetOrRplaceVideo(FileResult video)
    {
        if (video != null)
        {
            if (submittingVideo != null)
            {
                var replace = await DisplayAlert("Overwrite", "You already selected a video. Do you want to replace it?", "Yes", "No");

                if (replace)
                {
                    submittingVideo = video;
                    PublishVideo.IsEnabled = true;

                }
            }
            else
            {
                submittingVideo = video;
                PublishVideo.IsEnabled = true;
            }
        }
    }
}