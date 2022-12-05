// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Threading.Tasks;

namespace Extension_Packager.src.Controls
{
    public interface IPicker
    {
        public Task<string> GetPathDialog(string fileTypes);
    }
}
