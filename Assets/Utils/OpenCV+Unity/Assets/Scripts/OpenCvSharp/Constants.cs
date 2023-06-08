using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenCvSharp
{
    class Constants
    {
        //UFIX
#if !UNITY_EDITOR && UNITY_IOS
		public const string DllExtern = "__Internal";
#else
        public const string DllExtern = "OpenCvSharpExtern";
#endif
        public const string Version = "320";
    }
}
