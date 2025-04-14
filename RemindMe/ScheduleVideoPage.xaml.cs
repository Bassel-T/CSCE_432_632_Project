namespace RemindMe;

public partial class ScheduleVideoPage : ContentPage
{
	public ScheduleVideoPage()
	{
		InitializeComponent();
	}

    private async void ScheduleOrRecordVideo(object sender, EventArgs e)
    {
        await GenerateVideo();
    }

    private async Task GenerateVideo() { 
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
                    var newFile = Path.Combine(FileSystem.CacheDirectory, video.FileName);

                    using Stream inputStream = await video.OpenReadAsync();
                    using FileStream outputStream = File.OpenWrite(newFile);
                    await inputStream.CopyToAsync(outputStream);

                    await DisplayAlert("Video Recorded", $"Saved to: {newFile}", "OK");
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
            await DisplayAlert("Video Selected", result.FileName, "OK");
        }
    }
}