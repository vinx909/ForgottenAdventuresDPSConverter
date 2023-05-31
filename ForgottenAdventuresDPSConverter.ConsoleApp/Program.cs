using ForgottenAdventuresDPSConverter.Core.Entities;
using ForgottenAdventuresDPSConverter.Core.Interfaces;
using ForgottenAdventuresDPSConverter.Core.Reports;
using ForgottenAdventuresDPSConverter.Core.Services;
using ForgottenAdventuresDPSConverter.FileRepository;
using Microsoft.Extensions.DependencyInjection;

namespace ForgottenAdventuresDPSConverter.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            IServiceProvider services = SetupDependencies(new ServiceCollection());

            ///*
            IFAFolderReader reader = services.GetRequiredService<IFAFolderReader>();
            IHTMLConverter htmlConverter = services.GetRequiredService<IHTMLConverter>();
            IRepository<FAFolder> repository = services.GetRequiredService<IRepository<FAFolder>>();

            Console.WriteLine("write path to the master folder");
            string? folderPath = Console.ReadLine();
            if(folderPath != null)
            {
                //FAFolderUpdateReport report = reader.UpdateFolders(folderPath).Result;

                FAFolderUpdateReport result = reader.UpdateFolders(folderPath).Result;
                Console.WriteLine(result.ToString(repository));
            }
            Console.WriteLine("done");

            /*
            Console.WriteLine("the subfolders are");
            foreach(FAFolder folder in subfolders)
            {
                Console.Write(folder.Name +"\r\n");
            }
            */

            htmlConverter.FAHTMLConvert(new List<FAFolder>(repository.GetAll().Result), @"C:\Users\Octavia\Desktop\FAtable.html");
            //*/
            /*
           IWallConverter wallConverter = services.GetService<IWallConverter>();

           Console.WriteLine("write path to the wall image to crop into a wall");
           string? filePath = Console.ReadLine();
           Console.WriteLine("write path to where and with what name the new wall should be saved");
           string? wallPath = Console.ReadLine();
           Console.WriteLine(filePath + "will be converted to a wall as " + wallPath);
           Console.WriteLine("conversion was successful: " + wallConverter.ConvertWall(filePath, wallPath).Result);
            */

            //WallsConverter.ConvertAllWalls(services.GetRequiredService<IWallConverter>());
        }

        static IServiceProvider SetupDependencies(IServiceCollection services)
        {
            services.AddScoped<IFAFolderReader, FolderReader>();
            services.AddScoped<IWallConverter, WallConverter>();
            services.AddScoped<IHTMLConverter, HTMLConverter>();

            services.AddScoped<IRepository<FAFolder>, ForgottenAdventuresDPSConverter.FileRepository.FAFolderRepository>();
            services.AddScoped<IFileRepositorySettings, FileRepositorySettings>();

            //services.AddDbContext<FATrackerDbContext>();

            return services.BuildServiceProvider();
        }
    }
}