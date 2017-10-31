using Newtonsoft.Json.Linq;
using ReactNative.Bridge;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;

namespace Com.Reactlibrary.RNReactNativeDocViewer
{
    /// <summary>
    /// A module that allows JS to share data.
    /// </summary>
    class RNReactNativeDocViewerModule : ReactContextNativeModuleBase, ILifecycleEventListener
    {
        /// <summary>
        /// Instantiates the <see cref="RNReactNativeDocViewerModule"/>.
        /// </summary>
        public RNReactNativeDocViewerModule(ReactContext reactContext)
            : base(reactContext)
        {
        }

        /// <summary>
        /// The name of the native module.
        /// </summary>
        public override string Name
        {
            get
            {
                return "RNReactNativeDocViewer";
            }
        }

        public override void Initialize()
        {
            Context.AddLifecycleEventListener(this);
        }

        public void OnSuspend()
        {
        }

        public void OnResume()
        {
        }

        public void OnDestroy()
        {
        }


        [ReactMethod]
        async void openDoc(JArray options, ICallback callback)
        {
            // Path to the file in the app package to launch
            String imageUrl = options.First.Value<String>("url");

            var file = await ApplicationData.Current.LocalFolder.GetFileAsync(System.IO.Path.GetFileName(imageUrl)).AsTask().ConfigureAwait(false);

            if (file != null)
            {
                RunOnDispatcher(new DispatchedHandler(async () =>
                {

                    var launcherOptions = new Windows.System.LauncherOptions();
                    launcherOptions.DisplayApplicationPicker = true;

                    // Launch the retrieved file
                    
                    var success = await Windows.System.Launcher.LaunchFileAsync(file, launcherOptions).AsTask().ConfigureAwait(false);

                    if (success)
                    {
                        // File launched
                        callback.Invoke(null, imageUrl);
                    }
                    else
                    {
                        // File launch failed
                        callback.Invoke("Error occured while launching file!", null);
                    }
                }));
            }
            else
            {
                // Could not find file
                callback.Invoke("File could not found!", null);
            }
        }

        private static async void RunOnDispatcher(DispatchedHandler action)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, action).AsTask().ConfigureAwait(false);
        }

    }
}
