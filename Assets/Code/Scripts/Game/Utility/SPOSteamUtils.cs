namespace SPO.Utility
{
    using UnityEngine;
    using Steamworks;

    public static class SPOSteamUtils
    {
        /// <summary>
        /// Gets the Steam avatar image of a player.
        /// </summary>
        /// <param name="iImage">The image id of the player.</param>
        /// <returns>The Steam avatar image of a player as a texture 2D.</returns>
        public static Texture2D GetSteamAvatar(int iImage)
        {
            Texture2D ret = null;
            uint ImageWidth;
            uint ImageHeight;
            bool bIsValid = SteamUtils.GetImageSize(iImage, out ImageWidth, out ImageHeight);

            if (bIsValid)
            {
                byte[] Image = new byte[ImageWidth * ImageHeight * 4];

                bIsValid = SteamUtils.GetImageRGBA(iImage, Image, (int)(ImageWidth * ImageHeight * 4));
                if (bIsValid)
                {
                    ret = new Texture2D((int)ImageWidth, (int)ImageHeight, TextureFormat.RGBA32, false, true);
                    ret.LoadRawTextureData(Image);
                    ret.Apply();
                }
            }

            return ret;
        }
    }
}