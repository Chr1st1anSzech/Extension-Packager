// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Extension_Packager_Library.src.Helper
{
    public class Uri
    {
        public static string Combine(string path1, params string[] paths)
        {
            foreach (string path2 in paths)
            {
                path1 = $"{path1.TrimEnd('/')}/{path2.TrimStart('/')}";
            }
            return path1;
        }
    }
}
