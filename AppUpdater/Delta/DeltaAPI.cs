using System;

namespace AppUpdater.Delta
{
    public static class DeltaAPI
    {
        private static bool checkedSupport = false;
        private static bool isSupported = false;

        public static void CreateDelta(string originalFile, string newFile, string deltaFile, bool isExecutable)
        {
            DELTA_INPUT options = new DELTA_INPUT();

            long fileType = isExecutable ? NativeConstants.DELTA_FILE_TYPE_SET_EXECUTABLES : NativeConstants.DELTA_FILE_TYPE_RAW;
            
            NativeMethods.CreateDeltaW(
                   NativeConstants.DELTA_FILE_TYPE_SET_EXECUTABLES,
                   NativeConstants.DELTA_FLAG_NONE,
                   NativeConstants.DELTA_FLAG_NONE,
                   originalFile,
                   newFile,
                   null,
                   null,
                   options,
                   IntPtr.Zero,
                   0,
                   deltaFile);
        }

        public static void ApplyDelta(string originalFile, string newFile, string deltaFile)
        {
            NativeMethods.ApplyDeltaW(
                    NativeConstants.DELTA_FLAG_NONE,
                    originalFile,
                    deltaFile,
                    newFile);
        }

        public static bool IsSupported()
        {
            if (!checkedSupport)
            {
                isSupported = CheckApiSupport();
                checkedSupport = true;
            }

            return isSupported;
        }

        private static bool CheckApiSupport()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
                Environment.OSVersion.Version.Major >= 6)
            {
                // Windows Vista or later
                return true;
            }

            try
            {
                // Try to call the API.
                DELTA_INPUT options = new DELTA_INPUT();
                NativeMethods.CreateDeltaW(
                   NativeConstants.DELTA_FILE_TYPE_SET_EXECUTABLES,
                   NativeConstants.DELTA_FLAG_NONE,
                   NativeConstants.DELTA_FLAG_NONE,
                   null,
                   null,
                   null,
                   null,
                   options,
                   IntPtr.Zero,
                   0,
                   null);

                // The API is supported.
                return true;
            }
            catch (DllNotFoundException)
            {
                // Unable to load DLL 'msdelta.dll'
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
