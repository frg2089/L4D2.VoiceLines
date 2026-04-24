using Microsoft.AI.Foundry.Local;
using Microsoft.Extensions.Logging.Abstractions;

internal sealed class Whisper
{
    public static async Task<OpenAIAudioClient> InitializeAsync()
    {
        var config = new Configuration
        {
            AppName = "foundry_local_samples",
            LogLevel = LogLevel.Information
        };


        // Initialize the singleton instance.
        await FoundryLocalManager.CreateAsync(config, NullLogger.Instance);
        var mgr = FoundryLocalManager.Instance;


        // Ensure that any Execution Provider (EP) downloads run and are completed.
        // Download and register all execution providers.
        var currentEp = "";
        await mgr.DownloadAndRegisterEpsAsync((epName, percent) =>
        {
            if (epName != currentEp)
            {
                if (currentEp != "") Console.WriteLine();
                currentEp = epName;
            }
            Console.Write($"\r  {epName,-30}  {percent,6:F1}%");
        });
        if (currentEp != "") Console.WriteLine();


        // Get the model catalog
        var catalog = await mgr.GetCatalogAsync();


        // Get a model using an alias and select the CPU model variant
        var model = await catalog.GetModelAsync("whisper-tiny") ?? throw new System.Exception("Model not found");
        var modelVariant = model.Variants.First(v => v.Info.Runtime?.DeviceType == DeviceType.CPU);
        model.SelectVariant(modelVariant);


        // Download the model (the method skips download if already cached)
        await model.DownloadAsync(progress =>
        {
            Console.Write($"\rDownloading model: {progress:F2}%");
            if (progress >= 100f)
            {
                Console.WriteLine();
            }
        });


        // Load the model
        Console.Write($"Loading model {model.Id}...");
        await model.LoadAsync();
        Console.WriteLine("done.");


        // Get an audio client
        var audioClient = await model.GetAudioClientAsync();
        audioClient.Settings.Language = "en";

        return audioClient;
    }
}