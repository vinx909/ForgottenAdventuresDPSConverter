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
            IFAFolderService reader = services.GetRequiredService<IFAFolderService>();
            IHtmlConverter htmlConverter = services.GetRequiredService<IHtmlConverter>();
            IRepository<FAFolder> repository = services.GetRequiredService<IRepository<FAFolder>>();

            Console.WriteLine("write path to the master folder");
            string? folderPath = Console.ReadLine();
            
            

            if(folderPath != null)
            {
                ProgressBar progressBar = new(Console.GetCursorPosition(), 200);
                Progress<double> progress = new(progressBar.Report);

                Console.WriteLine("\r\ntest line");

                FAFolderUpdateReport result = reader.UpdateFolders(folderPath, progress).Result;
                Console.WriteLine(result.Summery());
            }
            Console.WriteLine("done");

            Console.Read();
            htmlConverter.FAHtmlConvert(new List<FAFolder>(repository.GetAll().Result), @"C:\Users\Octavia\Desktop\FAtable.html");
        }

        static IServiceProvider SetupDependencies(IServiceCollection services)
        {
            services
                .AddScoped<IDpsFolderService, DpsFolderService>()
                .AddScoped<IDpsNumberService, DpsNumberService>()
                .AddScoped<IDpsSubfolderService, DpsSubfolderService>()
                .AddScoped<IFAFolderService, FAFolderService>()
                .AddScoped<IHtmlConverter, HtmlConverter>()
                .AddScoped<IWallConverter, WallConverter>();

            services
                .AddScoped<IFileRepositorySettings, FileRepositorySettings>()
                .AddScoped<IRepository<DpsFolder>, ForgottenAdventuresDPSConverter.FileRepository.DpsFolderFileRepository>()
                .AddScoped<IRepository<DpsNumber>, ForgottenAdventuresDPSConverter.FileRepository.DpsNumberFileRepository>()
                .AddScoped<IRepository<DpsSubfolder>, ForgottenAdventuresDPSConverter.FileRepository.DpsSubfolderFileRepository>()
                .AddScoped<IRepository<FAFolder>, ForgottenAdventuresDPSConverter.FileRepository.FAFolderFileRepository>();

            //services.AddDbContext<FATrackerDbContext>();

            return services.BuildServiceProvider();
        }

        class ProgressBar : IProgress<double>
        {
            private const char loaded = '*';
            private const char notLoaded = '-';
            private const int defaultLength = 30;

            private readonly int cursorPositionLeft;
            private readonly int cursorPositionTop;
            private readonly int length;

            private double maxNLoaded = 0;

            public ProgressBar((int, int) cursorPosition, int length)
            {
                this.cursorPositionLeft = cursorPosition.Item1;
                this.cursorPositionTop = cursorPosition.Item2;
                this.length = length;

                maxNLoaded = maxNLoaded - 1;
                Report(0);
            }

            public ProgressBar((int, int) cursorPosition)
            {
                this.cursorPositionLeft = cursorPosition.Item1;
                this.cursorPositionTop = cursorPosition.Item2;
                this.length = defaultLength;

                maxNLoaded = maxNLoaded - 1;
                Report(0);
            }

            public void Report(double value)
            {
                int nLoaded = (int)Math.Round(value * length);
                if(nLoaded <= maxNLoaded)
                {
                    return;
                }
                maxNLoaded = Math.Max(nLoaded, maxNLoaded);

                (int, int) cursorPosition = Console.GetCursorPosition();

                string barString = string.Empty;
                for (int i = 1; i <= length; i++)
                {
                    if (i <= nLoaded)
                    {
                        barString += loaded;
                    }
                    else
                    {
                        barString += notLoaded;
                    }
                }

                if (nLoaded >= maxNLoaded)
                {
                    maxNLoaded = Math.Max(nLoaded, maxNLoaded);

                    lock (this)
                    {
                        Console.SetCursorPosition(cursorPositionLeft, cursorPositionTop);
                        Console.Write(barString);
                        Console.SetCursorPosition(cursorPosition.Item1, cursorPosition.Item2);
                    }
                    
                }
            }
        }

    }
}