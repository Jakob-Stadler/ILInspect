namespace ILInspect {

    internal static class Program {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args) {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            MessageCollection messageCollection = new();

            const string DefaultConfig = "./data/config.json";
            string configPath;
            if (args.Length > 0) {
                configPath = Path.GetFullPath(args[0], Environment.CurrentDirectory);
            } else {
                configPath = Path.GetFullPath(DefaultConfig, AppDomain.CurrentDomain.BaseDirectory);
            }

            // Create GUI instance
            MainWindow mainWindow = new(configPath, messageCollection);

            // Display and run GUI
            Application.Run(mainWindow);
        }
    }
}