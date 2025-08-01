﻿using System;
using Avalonia;
using Velopack;

namespace RecipeApp
{
    internal sealed class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                VelopackApp.Build()
                    .SetAutoApplyOnStartup(true)
                    .Run();

                BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

            }
            catch (Exception ex)
            {
                string message = "Unhandled exception: " + ex.ToString();
                Console.WriteLine(message);
                throw;
            }
        }


        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();
    }
}
