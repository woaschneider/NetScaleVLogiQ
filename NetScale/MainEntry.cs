using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.GLOBAL;
using OakLeaf.MM.Main;
using OakLeaf.MM.Main.WPF;

namespace HWB.NETSCALE.FRONTEND.WPF
{
    /// <summary>
    /// Main class
    /// </summary>
    public class MainEntry
    {
        /// <summary>
        /// Specifies if this is a single instance application
        /// </summary>
        private static bool IsSingleInstance = false;

        /// <summary>
        /// Specifies if Windows are shown in the taskbar by default
        /// </summary>
        private static bool ShowWindowsInTaskbarDefault = false;

        /// <summary>
        /// App Property
        /// </summary>
        public static mmAppWPF App
        {
            get { return _app; }
            set { _app = value; }
        }

        private static mmAppWPF _app;

        /// <summary>
        /// Main application Window
        /// </summary>
        private static MainWindow mainWindow;

        [STAThread]
        public static void Main(string[] args)
        {
            // Instantiate the Application object
            App = new AppWPF();

            // Set the ShowInTaskbar default
            mmAppWPF.WindowManager.ShowInTaskbarDefault = ShowWindowsInTaskbarDefault;

            bool? Result = true;

            //// Display the login window
            //   UserLoginWindow LoginWindow = new UserLoginWindow();
            try
            {
                Einstellungen boE = new Einstellungen();
                var boListe = boE.GetEinstellungen();
                if (boListe == null)
                {
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Verbindungstest zum SQL Server");
                return;
            }


            MyLoginWindow LoginWindow = new MyLoginWindow();
            Result = LoginWindow.ShowDialog();


            if (Result == true)
            {
                App app = new App();
                app.InitializeComponent();
                // Instantiate the main application window
                mainWindow = new MainWindow();
                mmApplicationWrapper appWrapper = new mmApplicationWrapper(app, mainWindow, IsSingleInstance);

                // Log any unhandled exceptions
                appWrapper.Application.DispatcherUnhandledException +=
                    delegate(object sender, DispatcherUnhandledExceptionEventArgs e)
                        {
                            // Write the error to the application log
                            mmAppBase.Log.WriteException(e.Exception);

                            // Display the Exception form
                            mmExceptionWindow ExceptionWindow = new mmExceptionWindow(e.Exception.Message,
                                                                                      e.Exception.StackTrace);
                            ExceptionWindow.ShowDialog();
                        };

                // If IsSingleInstance is true, activates the current instance of the 
                // application rather than running multiple instances
                appWrapper.StartupNextInstance += delegate
                                                      {
                                                          // If the window is currently minimized, then restore it.
                                                          if (mainWindow.WindowState == WindowState.Minimized)
                                                          {
                                                              mainWindow.WindowState = WindowState.Normal;
                                                          }

                                                          // Activate the current instance of the app, so that it's shown.
                                                          mainWindow.Activate();
                                                      };

                appWrapper.Run(args);
            }
            else
            {
                MessageBox.Show("Benutzer und/oder Passwort sind nicht bekannt! Der Programmstart wird abgebrochen!",
                                "ACHTUNG",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}