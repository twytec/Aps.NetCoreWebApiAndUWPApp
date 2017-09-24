using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x407 dokumentiert.

namespace AppUWP
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        string ApiUrl = "http://localhost:59966/api/Data/";

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void BtnUploadFileAsJson_Click(object sender, RoutedEventArgs e)
        {
            Windows.Storage.Pickers.FileOpenPicker fileOpenPicker = new Windows.Storage.Pickers.FileOpenPicker() {
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop,
                ViewMode = Windows.Storage.Pickers.PickerViewMode.List
            };
            fileOpenPicker.FileTypeFilter.Add(".pdf");
            fileOpenPicker.FileTypeFilter.Add(".jpg");

            var file = await fileOpenPicker.PickSingleFileAsync();

            if (file != null)
            {
                SharedModels.FileDataModel model = new SharedModels.FileDataModel();

                switch (file.FileType)
                {
                    case ".pdf":
                        model.MimeType = "application/pdf";
                        break;
                    case ".jpg":
                        model.MimeType = "image/jpeg";
                        break;
                }

                model.Extension = file.FileType;

                var fileBytes = await StorageFileToByteArray(file);
                model.Base64String = ByteArrayToBase64(fileBytes);
                model.FileName = file.Name;

                using (var client = new HttpClient())
                {
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(model);

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent theContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                    try
                    {
                        var response = await client.PostAsync(new Uri($"{ApiUrl}UploadFileAsJson"), theContent);
                    }
                    catch (Exception)
                    {
                        
                    }

                }
            }
        }

        private async void BtnUploadAsByteArray_Click(object sender, RoutedEventArgs e)
        {
            Windows.Storage.Pickers.FileOpenPicker fileOpenPicker = new Windows.Storage.Pickers.FileOpenPicker()
            {
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop,
                ViewMode = Windows.Storage.Pickers.PickerViewMode.List
            };
            fileOpenPicker.FileTypeFilter.Add(".pdf");
            fileOpenPicker.FileTypeFilter.Add(".jpg");

            var file = await fileOpenPicker.PickSingleFileAsync();

            if (file != null)
            {
                var fileBytes = await StorageFileToByteArray(file);
                string mimeType = "";

                switch (file.FileType)
                {
                    case ".pdf":
                        mimeType = "application/pdf";
                        break;
                    case ".jpg":
                        mimeType = "image/jpeg";
                        break;
                }

                using (var client = new HttpClient())
                {
                    MultipartFormDataContent multipartForm = new MultipartFormDataContent();

                    var byteContent = new ByteArrayContent(fileBytes);
                    byteContent.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(mimeType);

                    multipartForm.Add(byteContent, file.DisplayName, file.Name);
                    

                    try
                    {
                        var response = await client.PostAsync(new Uri($"{ApiUrl}FileUploadAsByteArray"), multipartForm);
                    }
                    catch (Exception)
                    {

                    }

                }

            }
        }

        private async void BtnGetModelFromApi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    
                    var json = await client.GetStringAsync($"{ApiUrl}GetFileData");
                    var model = Newtonsoft.Json.JsonConvert.DeserializeObject<SharedModels.FileDataModel>(json);
                }
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        private async void BtnDownloadFileFromApi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    var stream = await client.GetStreamAsync($"{ApiUrl}DownloadFile/Text.txt");
                    
                }
            }
            catch (Exception)
            {

                throw;
            }


        }

        #region Helper

        async Task<byte[]> StorageFileToByteArray(Windows.Storage.StorageFile file)
        {
            byte[] fileBytes = null;

            using (var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
            {
                fileBytes = new byte[stream.Size];
                using (var reader = new Windows.Storage.Streams.DataReader(stream))
                {
                    await reader.LoadAsync((uint)stream.Size);
                    reader.ReadBytes(fileBytes);
                }
            }

            return fileBytes;
        }

        string ByteArrayToBase64(byte[] fileBytes)
        {
            return Convert.ToBase64String(fileBytes);
        }

        #endregion

        
    }
}
