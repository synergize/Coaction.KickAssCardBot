namespace Coaction.KickAssCardBot.Constants
{
    /// <summary>
    /// Contains constant values for MTG card layout types.
    /// </summary>
    public static class LayoutConstants
    {
        /// <summary>
        /// Split card layout (two different spells on one card).
        /// </summary>
        public const string Split = "split";

        /// <summary>
        /// Transform layout (card that flips to show different face).
        /// </summary>
        public const string Transform = "transform";

        /// <summary>
        /// Modal double-faced card layout (choose one face).
        /// </summary>
        public const string ModalDfc = "modal_dfc";

        /// <summary>
        /// Adventure layout (spell with adventure cost).
        /// </summary>
        public const string Adventure = "adventure";

        /// <summary>
        /// Prepare layout (card with prepare mechanic).
        /// </summary>
        public const string Prepare = "prepare";

        public const string Flip = "flip";
    }
}
