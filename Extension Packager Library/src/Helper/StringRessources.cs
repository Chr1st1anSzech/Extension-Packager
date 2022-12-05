// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Extension_Packager_Library.src.Helper
{
    public class StringResources
    {
        public static string Get(object callingClass, int number, params object[] parameters)
        {
            if (callingClass is null)
            {
                throw new ArgumentNullException(nameof(callingClass));
            }
            var resourceContext = new Windows.ApplicationModel.Resources.Core.ResourceContext();
            var resourceMap = Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap.GetSubtree("Resources");
            string id = $"{callingClass.GetType().Name}{number}";
            var txt = resourceMap.GetValue(id, resourceContext)?.ValueAsString ?? "";
            if (parameters != null)
            {
                txt = string.Format(txt, parameters);
            }
            return txt;
        }

        public static string GetWithReason(object callingClass, int number, string reason)
        {
            if (callingClass is null)
            {
                throw new ArgumentNullException(nameof(callingClass));
            }
            var resourceContext = new Windows.ApplicationModel.Resources.Core.ResourceContext();
            var resourceMap = Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap.GetSubtree("Resources");
            string id = $"{callingClass.GetType().Name}{number}";
            var txt = resourceMap.GetValue(id, resourceContext)?.ValueAsString ?? "";
            if (reason != null)
            {
                txt = $"{txt}\n({reason})";
            }
            return txt;
        }
    }
}
