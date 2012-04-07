using System;

namespace AppUpdater.Delta
{
    public static class DeltaAPI
    {
        public static bool IsSupported()
        {
            // TODO: implement it and test on Windows XP
            return true;
        }

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
    }
}
