// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Localisation;

namespace osu.Game.Localisation
{
    public static class DeleteConfirmationDialogStrings
    {
        private const string prefix = @"osu.Game.Resources.Localisation.DeleteConfirmationDialog";

        /// <summary>
        /// "Confirm deletion of"
        /// </summary>
        public static LocalisableString HeaderText => new TranslatableString(getKey(@"header_text"), @"Confirm deletion of");

        /// <summary>
        /// "确定"
        /// </summary>
        public static LocalisableString Confirm => new TranslatableString(getKey(@"confirm"), @"确定");

        /// <summary>
        /// "取消"
        /// </summary>
        public static LocalisableString Cancel => new TranslatableString(getKey(@"cancel"), @"取消");

        private static string getKey(string key) => $@"{prefix}:{key}";
    }
}
