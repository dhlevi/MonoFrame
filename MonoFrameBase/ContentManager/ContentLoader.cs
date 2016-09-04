using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoFrame.ContentManager
{
    /// <summary>
    /// Content loader helper. This class is used by the content manager to assist with loading 
    /// various asset types from the content pipline
    /// </summary>
    public static class ContentLoader
    {
        public static void LoadSpriteFont(Game game, string font)
        {
            if (!string.IsNullOrEmpty(font))
            {
                ContentResourceManager.Instance.Fonts.Add(new ContentResource<SpriteFont>(game.Content.Load<SpriteFont>(font), font));
            }
        }

        public static void LoadTexture(Game game, string texture)
        {
            if (!string.IsNullOrEmpty(texture))
            {
                ContentResourceManager.Instance.Textures.Add(new ContentResource<Texture>(game.Content.Load<Texture>(texture), texture));
            }
        }

        public static void LoadTexture2D(Game game, string texture)
        {
            if (!string.IsNullOrEmpty(texture)) 
            {
                ContentResourceManager.Instance.Texture2Ds.Add(new ContentResource<Texture2D>(game.Content.Load<Texture2D>(texture), texture));
            }
        }

        public static void LoadTexture3D(Game game, string texture)
        {
            if (!string.IsNullOrEmpty(texture))
            {
                ContentResourceManager.Instance.Texture3Ds.Add(new ContentResource<Texture3D>(game.Content.Load<Texture3D>(texture), texture));
            }
        }

        public static void LoadModel(Game game, string model)
        {
            if (!string.IsNullOrEmpty(model))
            {
                ContentResourceManager.Instance.Models.Add(new ContentResource<Model>(game.Content.Load<Model>(model), model));
            }
        }

        public static void LoadEffect(Game game, string effect)
        {
            if (!string.IsNullOrEmpty(effect))
            {
                ContentResourceManager.Instance.Effects.Add(new ContentResource<Effect>(game.Content.Load<Effect>(effect), effect));
            }
        }
    }
}
