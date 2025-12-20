// Example: Windows camera access using Windows.Media.Capture
// Note: This sample targets UWP/WinUI where Windows Runtime APIs are available.
// Required capabilities in Package.appxmanifest: <DeviceCapability Name="webcam" />

using System;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;

namespace CameraSample
{
    public class CameraCapture
    {
        public async Task<StorageFile> CapturePhotoAsync()
        {
            var capture = new MediaCapture();

            // Initialize camera
            await capture.InitializeAsync();

            // Create a JPEG file in Pictures library
            StorageFile file = await KnownFolders.PicturesLibrary
                .CreateFileAsync($"photo_{DateTime.UtcNow:yyyyMMdd_HHmmss}.jpg");

            // Set JPEG encoding
            ImageEncodingProperties imageProperties = ImageEncodingProperties.CreateJpeg();

            // Capture photo to file
            await capture.CapturePhotoToStorageFileAsync(imageProperties, file);

            return file;
        }
    }
}
