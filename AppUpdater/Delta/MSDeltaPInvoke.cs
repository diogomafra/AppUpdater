using System;
using System.Runtime.InteropServices;

namespace AppUpdater.Delta
{
    public partial class NativeConstants
    {
        public const int DELTA_FILE_SIZE_LIMIT = (32 * (1024 * 1024));
        public const int DELTA_OPTIONS_SIZE_LIMIT = (128 * (1024 * 1024));
        public const Int64 DELTA_FILE_TYPE_RAW = 0x00000001;
        public const Int64 DELTA_FILE_TYPE_I386 = 0x00000002;
        public const Int64 DELTA_FILE_TYPE_IA64 = 0x00000004;
        public const Int64 DELTA_FILE_TYPE_AMD64 = 0x00000008;
        public const Int64 DELTA_FILE_TYPE_SET_RAW_ONLY = NativeConstants.DELTA_FILE_TYPE_RAW;
        public const Int64 DELTA_FILE_TYPE_SET_EXECUTABLES = (NativeConstants.DELTA_FILE_TYPE_RAW
                    | (NativeConstants.DELTA_FILE_TYPE_I386
                    | (NativeConstants.DELTA_FILE_TYPE_IA64 | NativeConstants.DELTA_FILE_TYPE_AMD64)));
        public const Int64 DELTA_FLAG_NONE = 0x00000000;
        public const Int64 DELTA_APPLY_FLAG_ALLOW_PA19 = 0x00000001;
        public const Int64 DELTA_FLAG_E8 = 0x00000001;
        public const Int64 DELTA_FLAG_MARK = 0x00000002;
        public const Int64 DELTA_FLAG_IMPORTS = 0x00000004;
        public const Int64 DELTA_FLAG_EXPORTS = 0x00000008;
        public const Int64 DELTA_FLAG_RESOURCES = 0x00000010;
        public const Int64 DELTA_FLAG_RELOCS = 0x00000020;
        public const Int64 DELTA_FLAG_I386_SMASHLOCK = 0x00000040;
        public const Int64 DELTA_FLAG_I386_JMPS = 0x00000080;
        public const Int64 DELTA_FLAG_I386_CALLS = 0x00000100;
        public const Int64 DELTA_FLAG_AMD64_DISASM = 0x00000200;
        public const Int64 DELTA_FLAG_AMD64_PDATA = 0x00000400;
        public const Int64 DELTA_FLAG_IA64_DISASM = 0x00000800;
        public const Int64 DELTA_FLAG_IA64_PDATA = 0x00001000;
        public const Int64 DELTA_FLAG_UNBIND = 0x00002000;
        public const Int64 DELTA_FLAG_CLI_DISASM = 0x00004000;
        public const Int64 DELTA_FLAG_CLI_METADATA = 0x00008000;
        public const Int64 DELTA_FLAG_HEADERS = 0x00010000;
        public const Int64 DELTA_FLAG_IGNORE_FILE_SIZE_LIMIT = 0x00020000;
        public const Int64 DELTA_FLAG_IGNORE_OPTIONS_SIZE_LIMIT = 0x00040000;
        public const Int64 DELTA_DEFAULT_FLAGS_RAW = NativeConstants.DELTA_FLAG_NONE;
        public const Int64 DELTA_DEFAULT_FLAGS_I386 = (NativeConstants.DELTA_FLAG_MARK
                    | (NativeConstants.DELTA_FLAG_IMPORTS
                    | (NativeConstants.DELTA_FLAG_EXPORTS
                    | (NativeConstants.DELTA_FLAG_RESOURCES
                    | (NativeConstants.DELTA_FLAG_RELOCS
                    | (NativeConstants.DELTA_FLAG_I386_SMASHLOCK
                    | (NativeConstants.DELTA_FLAG_I386_JMPS
                    | (NativeConstants.DELTA_FLAG_I386_CALLS
                    | (NativeConstants.DELTA_FLAG_UNBIND
                    | (NativeConstants.DELTA_FLAG_CLI_DISASM | NativeConstants.DELTA_FLAG_CLI_METADATA))))))))));
        public const Int64 DELTA_DEFAULT_FLAGS_IA64 = (NativeConstants.DELTA_FLAG_MARK
                    | (NativeConstants.DELTA_FLAG_IMPORTS
                    | (NativeConstants.DELTA_FLAG_EXPORTS
                    | (NativeConstants.DELTA_FLAG_RESOURCES
                    | (NativeConstants.DELTA_FLAG_RELOCS
                    | (NativeConstants.DELTA_FLAG_IA64_DISASM
                    | (NativeConstants.DELTA_FLAG_IA64_PDATA
                    | (NativeConstants.DELTA_FLAG_UNBIND
                    | (NativeConstants.DELTA_FLAG_CLI_DISASM | NativeConstants.DELTA_FLAG_CLI_METADATA)))))))));
        public const Int64 DELTA_DEFAULT_FLAGS_AMD64 = (NativeConstants.DELTA_FLAG_MARK
                    | (NativeConstants.DELTA_FLAG_IMPORTS
                    | (NativeConstants.DELTA_FLAG_EXPORTS
                    | (NativeConstants.DELTA_FLAG_RESOURCES
                    | (NativeConstants.DELTA_FLAG_RELOCS
                    | (NativeConstants.DELTA_FLAG_AMD64_DISASM
                    | (NativeConstants.DELTA_FLAG_AMD64_PDATA
                    | (NativeConstants.DELTA_FLAG_UNBIND
                    | (NativeConstants.DELTA_FLAG_CLI_DISASM | NativeConstants.DELTA_FLAG_CLI_METADATA)))))))));
        public const int DELTA_MAX_HASH_SIZE = 32;
    }

    #if USE_ALL

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct DELTA_OUTPUT
    {
        public System.IntPtr lpStart;

        public uint uSize;
    }

    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct DELTA_HASH
    {
        public uint HashSize;

        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string HashValue;
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct DELTA_HEADER_INFO
    {
        public long FileTypeSet;
        public long FileType;
        public long Flags;
        public uint TargetSize;
        public FILETIME TargetFileTime;
        public uint TargetHashAlgId;
        public DELTA_HASH TargetHash;
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct FILETIME
    {
        public uint dwLowDateTime;
        public uint dwHighDateTime;
    }

    #endif

    [StructLayoutAttribute(LayoutKind.Explicit)]
    public struct Anonymous_5f2a119c_cb15_4d4e_86b9_c244a4a45396
    {
        [FieldOffsetAttribute(0)]
        public System.IntPtr lpcStart;

        [FieldOffsetAttribute(0)]
        public System.IntPtr lpStart;
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct DELTA_INPUT
    {
        public Anonymous_5f2a119c_cb15_4d4e_86b9_c244a4a45396 Union1;

        public uint uSize;

        [MarshalAsAttribute(UnmanagedType.Bool)]
        public bool Editable;
    }

    public class NativeMethods
    {
        [DllImportAttribute("msdelta.dll", EntryPoint = "ApplyDeltaW")]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        public static extern bool ApplyDeltaW(long ApplyFlags, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPWStr)] string lpSourceName, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPWStr)] string lpDeltaName, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPWStr)] string lpTargetName);

        [DllImportAttribute("msdelta.dll", EntryPoint = "CreateDeltaW")]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        public static extern bool CreateDeltaW(long FileTypeSet, long SetFlags, long ResetFlags, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPWStr)] string lpSourceName, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPWStr)] string lpTargetName, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPWStr)] string lpSourceOptionsName, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPWStr)] string lpTargetOptionsName, DELTA_INPUT GlobalOptions, [InAttribute()] System.IntPtr lpTargetFileTime, uint HashAlgId, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPWStr)] string lpDeltaName);

        #if USE_ALL

        [DllImportAttribute("msdelta.dll", EntryPoint = "GetDeltaInfoB")]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        public static extern bool GetDeltaInfoB(DELTA_INPUT Delta, [OutAttribute()] out DELTA_HEADER_INFO lpHeaderInfo);

        [DllImportAttribute("msdelta.dll", EntryPoint = "GetDeltaInfoA")]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        public static extern bool GetDeltaInfoA([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string lpDeltaName, [OutAttribute()] out DELTA_HEADER_INFO lpHeaderInfo);

        [DllImportAttribute("msdelta.dll", EntryPoint = "GetDeltaInfoW")]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        public static extern bool GetDeltaInfoW([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPWStr)] string lpDeltaName, [OutAttribute()] out DELTA_HEADER_INFO lpHeaderInfo);

        [DllImportAttribute("msdelta.dll", EntryPoint = "ApplyDeltaB")]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        public static extern bool ApplyDeltaB(long ApplyFlags, DELTA_INPUT Source, DELTA_INPUT Delta, [OutAttribute()] out DELTA_OUTPUT lpTarget);

        [DllImportAttribute("msdelta.dll", EntryPoint = "ApplyDeltaProvidedB")]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        public static extern bool ApplyDeltaProvidedB(long ApplyFlags, DELTA_INPUT Source, DELTA_INPUT Delta, System.IntPtr lpTarget, uint uTargetSize);

        [DllImportAttribute("msdelta.dll", EntryPoint = "ApplyDeltaA")]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        public static extern bool ApplyDeltaA(long ApplyFlags, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string lpSourceName, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string lpDeltaName, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string lpTargetName);

        [DllImportAttribute("msdelta.dll", EntryPoint = "CreateDeltaB")]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        public static extern bool CreateDeltaB(long FileTypeSet, long SetFlags, long ResetFlags, DELTA_INPUT Source, DELTA_INPUT Target, DELTA_INPUT SourceOptions, DELTA_INPUT TargetOptions, DELTA_INPUT GlobalOptions, [InAttribute()] System.IntPtr lpTargetFileTime, uint HashAlgId, [OutAttribute()] out DELTA_OUTPUT lpDelta);

        [DllImportAttribute("msdelta.dll", EntryPoint = "CreateDeltaA")]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        public static extern bool CreateDeltaA(long FileTypeSet, long SetFlags, long ResetFlags, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string lpSourceName, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string lpTargetName, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string lpSourceOptionsName, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string lpTargetOptionsName, DELTA_INPUT GlobalOptions, [InAttribute()] System.IntPtr lpTargetFileTime, uint HashAlgId, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string lpDeltaName);

        [DllImportAttribute("msdelta.dll", EntryPoint = "GetDeltaSignatureB")]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        public static extern bool GetDeltaSignatureB(long FileTypeSet, uint HashAlgId, DELTA_INPUT Source, [OutAttribute()] out DELTA_HASH lpHash);

        [DllImportAttribute("msdelta.dll", EntryPoint = "GetDeltaSignatureA")]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        public static extern bool GetDeltaSignatureA(long FileTypeSet, uint HashAlgId, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string lpSourceName, [OutAttribute()] out DELTA_HASH lpHash);

        [DllImportAttribute("msdelta.dll", EntryPoint = "GetDeltaSignatureW")]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        public static extern bool GetDeltaSignatureW(long FileTypeSet, uint HashAlgId, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPWStr)] string lpSourceName, [OutAttribute()] out DELTA_HASH lpHash);

        [DllImportAttribute("msdelta.dll", EntryPoint = "DeltaNormalizeProvidedB")]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        public static extern bool DeltaNormalizeProvidedB(long FileTypeSet, long NormalizeFlags, DELTA_INPUT NormalizeOptions, System.IntPtr lpSource, uint uSourceSize);

        [DllImportAttribute("msdelta.dll", EntryPoint = "DeltaFree")]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        public static extern bool DeltaFree([InAttribute()] System.IntPtr lpMemory);

        #endif
    }

}
