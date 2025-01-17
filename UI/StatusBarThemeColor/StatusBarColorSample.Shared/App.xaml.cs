using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace StatusBarColorSample
{
	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	public sealed partial class App : Application
	{
		private UISettings _uiSettings;
		private bool? _wasDarkMode = null;

		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App()
		{

			InitializeLogging();

#if __IOS__ || __ANDROID__
			// Enable native frame navigation.
			Uno.UI.FeatureConfiguration.Style.ConfigureNativeFrameNavigation();
#endif

			this.InitializeComponent();

#if HAS_UNO || NETFX_CORE
			this.Suspending += OnSuspending;
#endif

		}

		/// <summary>
		/// Invoked when the application is launched normally by the end user.  Other entry points
		/// will be used such as when the application is launched to open a specific file.
		/// </summary>
		/// <param name="e">Details about the launch request and process.</param>
		protected override void OnLaunched(LaunchActivatedEventArgs e)
		{
#if DEBUG
			if (System.Diagnostics.Debugger.IsAttached)
			{
				// this.DebugSettings.EnableFrameRateCounter = true;
			}
#endif

#if NET5_0 && WINDOWS
			var window = new Window();
			window.Activate();
#else
			var window = Windows.UI.Xaml.Window.Current;
#endif

			var rootFrame = window.Content as Frame;

			// Do not repeat app initialization when the Window already has content,
			// just ensure that the window is active
			if (rootFrame == null)
			{
				// Create a Frame to act as the navigation context and navigate to the first page
				rootFrame = new Frame();

				rootFrame.NavigationFailed += OnNavigationFailed;

				if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
				{
					//TODO: Load state from previously suspended application
				}

				// Place the frame in the current Window
				window.Content = rootFrame;
			}

#if !(NET5_0 && WINDOWS)
			if (e.PrelaunchActivated == false)
#endif
			{
				if (rootFrame.Content == null)
				{
					// When the navigation stack isn't restored navigate to the first page,
					// configuring the new page by passing required information as a navigation
					// parameter
					rootFrame.Navigate(typeof(MainPage), e.Arguments);
				}
				// Ensure the current window is active
				window.Activate();
			}

			ConfigureStatusBar();
		}

		private void ConfigureStatusBar()
		{
			// Listen for the system theme changes.
			_uiSettings = new UISettings();
			_uiSettings.ColorValuesChanged += (s, e) =>
			{
#if __ANDROID__
				var backgroundColor = _uiSettings.GetColorValue(UIColorType.Background);
				var isDarkMode = backgroundColor == Windows.UI.Colors.Black;

				// Prevent deadlock as setting StatusBar.ForegroundColor will also trigger this event.
				if (_wasDarkMode == isDarkMode) return;
				_wasDarkMode = isDarkMode;
#endif

				UpdateStatusBar();
			};

#if __IOS__
			// Force an update when the app is launched.
			UpdateStatusBar();
#endif
		}

		private void UpdateStatusBar()
		{
			// === 1. Determine the current theme from the background value, 
			// which is calculated from the theme and can only be black or white.
			var backgroundColor = _uiSettings.GetColorValue(UIColorType.Background);
			var isDarkMode = backgroundColor == Windows.UI.Colors.Black;

#if __IOS__ || __ANDROID__
			// === 2. Set the foreground color.
			// note: The foreground color can only be set to a "dark/light" value. See uno remarks on StatusBar.ForegroundColor.
			// note: For ios in dark mode, setting this value will have no effect.
			var foreground = isDarkMode ? Windows.UI.Colors.White : Windows.UI.Colors.Black;
			Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ForegroundColor = foreground;

			// === 3. Set the background color.
			var background = isDarkMode ? Windows.UI.Colors.MidnightBlue : Windows.UI.Colors.SkyBlue;
#if __ANDROID__
			// On Android, this is done by calling Window.SetStatusBarColor.
			if (Uno.UI.ContextHelper.Current is Android.App.Activity activity)
			{
				activity.Window.SetStatusBarColor(background);
			}
#endif
			// On iOS, this is done via the native CommandBar which goes under the status bar.
			// For android, we will also update the CommandBar just for consistency.
			if (MainPage.Instance.GetCommandBar() is CommandBar commandBar)
			{
				commandBar.Foreground = new SolidColorBrush(foreground); // controls the color for the "MainPage" page title
				commandBar.Background = new SolidColorBrush(background);
			}
#endif
		}

		/// <summary>
		/// Invoked when Navigation to a certain page fails
		/// </summary>
		/// <param name="sender">The Frame which failed navigation</param>
		/// <param name="e">Details about the navigation failure</param>
		void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
		{
			throw new Exception($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");
		}

		/// <summary>
		/// Invoked when application execution is being suspended.  Application state is saved
		/// without knowing whether the application will be terminated or resumed with the contents
		/// of memory still intact.
		/// </summary>
		/// <param name="sender">The source of the suspend request.</param>
		/// <param name="e">Details about the suspend request.</param>
		private void OnSuspending(object sender, SuspendingEventArgs e)
		{
			var deferral = e.SuspendingOperation.GetDeferral();
			//TODO: Save application state and stop any background activity
			deferral.Complete();
		}


		/// <summary>
		/// Configures global Uno Platform logging
		/// </summary>
		private static void InitializeLogging()
		{
			var factory = LoggerFactory.Create(builder =>
			{
#if __WASM__
                builder.AddProvider(new global::Uno.Extensions.Logging.WebAssembly.WebAssemblyConsoleLoggerProvider());
#elif __IOS__
                builder.AddProvider(new global::Uno.Extensions.Logging.OSLogLoggerProvider());
#elif NETFX_CORE
				builder.AddDebug();
#else
                builder.AddConsole();
#endif

				// Exclude logs below this level
				builder.SetMinimumLevel(LogLevel.Information);

				// Default filters for Uno Platform namespaces
				builder.AddFilter("Uno", LogLevel.Warning);
				builder.AddFilter("Windows", LogLevel.Warning);
				builder.AddFilter("Microsoft", LogLevel.Warning);

				// Generic Xaml events
				// builder.AddFilter("Windows.UI.Xaml", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.VisualStateGroup", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.StateTriggerBase", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.UIElement", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.FrameworkElement", LogLevel.Trace );

				// Layouter specific messages
				// builder.AddFilter("Windows.UI.Xaml.Controls", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.Controls.Layouter", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.Controls.Panel", LogLevel.Debug );

				// builder.AddFilter("Windows.Storage", LogLevel.Debug );

				// Binding related messages
				// builder.AddFilter("Windows.UI.Xaml.Data", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.Data", LogLevel.Debug );

				// Binder memory references tracking
				// builder.AddFilter("Uno.UI.DataBinding.BinderReferenceHolder", LogLevel.Debug );

				// RemoteControl and HotReload related
				// builder.AddFilter("Uno.UI.RemoteControl", LogLevel.Information);

				// Debug JS interop
				// builder.AddFilter("Uno.Foundation.WebAssemblyRuntime", LogLevel.Debug );
			});

			global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory = factory;

#if HAS_UNO
			global::Uno.UI.Adapter.Microsoft.Extensions.Logging.LoggingAdapter.Initialize();
#endif
		}
	}
}
