using WixSharp;
using WixSharp.CommonTasks;
using File = WixSharp.File;

namespace UpworkPdfGenerator.Installers;

internal class Program
{
    #region Constants

    private const string ApplicationName = "UpworkPdfGenerator";
    private const string CompanyName = "UpworkPdfGenerator";
    private const string RepositoryUrl = "https://github.com/HavenDV/UpworkPdfGenerator/";
    private const string Contact = "havendv@gmail.com";

    #endregion

    #region Main

    private static void Main()
    {
        try
        {
            CreateMsi();
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
        }
    }

    #endregion

    #region Methods

    private static void CreateMsi()
    {
        var project = new Project(
            ApplicationName,
            new Dir(
                @$"%ProgramFiles%\{CompanyName}\{ApplicationName}",
                new DirFiles(
                    @$"..\..\apps\UpworkPdfGenerator.Apps.Wpf\bin\Release\net4.6.1\*.*",
                    static value => !value.EndsWith(".exe")),
                new File(@"..\..\apps\UpworkPdfGenerator.Apps.Wpf\bin\Release\net4.6.1\UpworkPdfGenerator.Apps.Wpf.exe")
                {
                    Shortcuts = new[]
                    {
                        new FileShortcut(ApplicationName, "%ProgramMenu%"),
                        new FileShortcut(ApplicationName, "%Desktop%"),
                    }
                })
            //new LaunchApplicationFromExitDialog("EXE_ID", $"Launch {ApplicationName}")
            )
        {
            GUID = new Guid("2D943540-EF1F-43A1-AD8F-DA34E59CEB47"),
            ControlPanelInfo =
            {
                Manufacturer = CompanyName,
                ProductIcon = @"assets\icon.ico",
                Readme = $"{RepositoryUrl}blob/master/README.md",
                HelpLink = $"{RepositoryUrl}issues",
                UrlInfoAbout = $"{RepositoryUrl}blob/master/README.md",
                UrlUpdateInfo = $"{RepositoryUrl}blob/master/README.md",
                Contact = Contact,
            },
            UI = WUI.WixUI_ProgressOnly,
            Version = typeof(Program).Assembly.GetName().Version,
            MajorUpgrade = MajorUpgrade.Default,
            Properties = new[]
            {
                // https://documentation.help/Windows-Installer/msifastinstall.htm
                // Disables system restore point creation, some checks and progress messages.
                new Property("MSIFASTINSTALL", "7"),
            },
        }.SetNetFxPrerequisite(Condition.Net461_Installed, "Please install .NET 4.6.1 first.");

        Compiler.CandleOptions += " -nologo";
        Compiler.LightOptions += " -nologo";
        _ = Compiler.BuildMsi(project);
    }

    #endregion
}
