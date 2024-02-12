using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;


class Program
{
    static async Task Main(string[] args)
    {
        string connectionString = "DefaultEndpointsProtocol=https;AccountName=pavelaccount;AccountKey=YpD7LhCmBo/V3zCXj5z0IFGnj3zSEkRYURTLwfIA1WC/sSTQgROL53w2Vdr/HQErM6ex3ojJ3nkw+ASt/svCEw==;EndpointSuffix=core.windows.net";
        string containerName = "files"; 

        BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

        while (true)
        {
            Console.WriteLine("=====================================");
            Console.WriteLine("ADD");
            Console.WriteLine("Download");
            Console.WriteLine("Delete");
            Console.WriteLine("All");
            Console.WriteLine("=====================================");
            string ch = Console.ReadLine();
            switch (ch)
            {
                case "1":
                    await UploadFile(containerClient);
                    break;
                case "2":
                    await DownloadFile(containerClient);
                    break;
                case "3":
                    await DeleteFile(containerClient);
                    break;
                case "4":
                    await ListFiles(containerClient);
                    break;

                default:
                    break;
            }
        }
    }

    static async Task UploadFile(BlobContainerClient containerClient)
    {
        Console.WriteLine("Path:");
        string filePath = Console.ReadLine();
        string fileName = Path.GetFileName(filePath);
        BlobClient blobClient = containerClient.GetBlobClient(fileName);
        Console.WriteLine($"Upload {fileName} в Blob Storage...");
        using (FileStream fs = File.OpenRead(filePath))
        {
            await blobClient.UploadAsync(fs, true);
        }

        Console.WriteLine($"Ok.");
    }

    static async Task DownloadFile(BlobContainerClient containerClient)
    {
        Console.WriteLine("Name file:");
        string fileName = Console.ReadLine();

        BlobClient blobClient = containerClient.GetBlobClient(fileName);

        if (!await blobClient.ExistsAsync())
        {
            Console.WriteLine($"File {fileName} not exist.");
            return;
        }

        Console.WriteLine("Path to save:");
        string destinationPath = Console.ReadLine();
        string destinationFilePath = Path.Combine(destinationPath, fileName);
        Console.WriteLine($"Download {fileName}...");
        BlobDownloadInfo download = await blobClient.DownloadAsync();
        using (FileStream fs = File.OpenWrite(destinationFilePath))
        {
            await download.Content.CopyToAsync(fs);
        }
        Console.WriteLine($"{fileName} save to {destinationFilePath}.");
    }

    static async Task DeleteFile(BlobContainerClient containerClient)
    {
        Console.WriteLine("Input name deleting file:");
        string fileName = Console.ReadLine();

        BlobClient blobClient = containerClient.GetBlobClient(fileName);

        if (!await blobClient.ExistsAsync())
        {
            Console.WriteLine($"File {fileName} not exist.");
            return;
        }

        await blobClient.DeleteIfExistsAsync();

        Console.WriteLine($"file deleted");
    }

    static async Task ListFiles(BlobContainerClient containerClient)
    {
        Console.WriteLine("All files in Blob Storage:");

        await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
        {
            Console.WriteLine(blobItem.Name);
        }
    }
}
