using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MonoFrame.ContentManager
{
    /// <summary>
    /// The Content Resource manager singleton. This manager assists with the loading
    /// and holding of Models, textures, fonts and effects. Resources can be
    /// accessed by the singleton instance where needed
    /// </summary>
    public class ContentResourceManager
    {
        private static volatile ContentResourceManager instance;
        private static object syncRoot = new Object();

        public static ContentResourceManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        instance = new ContentResourceManager();
                    }
                }
                return instance;
            }
        }

        public List<ContentResource<Texture>> Textures { get; set; }
        public List<ContentResource<Texture2D>> Texture2Ds { get; set; }
        public List<ContentResource<Texture3D>> Texture3Ds { get; set; }
        public List<ContentResource<Model>> Models { get; set; }
        public List<ContentResource<Effect>> Effects { get; set; }
        public List<ContentResource<SpriteFont>> Fonts { get; set; }

        public ContentResourceManager() 
        {
            Textures = new List<ContentResource<Texture>>();
            Texture2Ds = new List<ContentResource<Texture2D>>();
            Texture3Ds = new List<ContentResource<Texture3D>>();
            Models = new List<ContentResource<Model>>();
            Effects = new List<ContentResource<Effect>>();
            Fonts = new List<ContentResource<SpriteFont>>();
        }

        public void Purge(Game game)
        {
            Textures.Clear();
            Texture2Ds.Clear();
            Texture3Ds.Clear();
            Models.Clear();
            Effects.Clear();
            Fonts.Clear();

            Textures = new List<ContentResource<Texture>>();
            Texture2Ds = new List<ContentResource<Texture2D>>();
            Texture3Ds = new List<ContentResource<Texture3D>>();
            Models = new List<ContentResource<Model>>();
            Effects = new List<ContentResource<Effect>>();
            Fonts = new List<ContentResource<SpriteFont>>();

            game.Content.Unload();
        }

        public SpriteFont GetFont(string resourceString)
        {
            if (Fonts.Count(res => res.ResourceString.Equals(resourceString)) == 1)
            {
                return Fonts.First(res => res.ResourceString.Equals(resourceString)).Content;
            }
            else return null;
        }

        public Texture GetTexture(string resourceString)
        {
            if (Textures.Count(res => res.ResourceString.Equals(resourceString)) == 1)
            {
                return Textures.First(res => res.ResourceString.Equals(resourceString)).Content;
            }
            else return null;
        }

        public Texture2D GetTexture2D(string resourceString)
        {
            if (Texture2Ds.Count(res => res.ResourceString.Equals(resourceString)) == 1)
            {
                return Texture2Ds.First(res => res.ResourceString.Equals(resourceString)).Content;
            }
            else return null;
        }

        public Texture3D GetTexture3D(string resourceString)
        {
            if (Texture3Ds.Count(res => res.ResourceString.Equals(resourceString)) == 1)
            {
                return Texture3Ds.First(res => res.ResourceString.Equals(resourceString)).Content;
            }
            else return null;
        }

        public Model GetModel(string resourceString)
        {
            if (Models.Count(res => res.ResourceString.Equals(resourceString)) == 1)
            {
                return Models.First(res => res.ResourceString.Equals(resourceString)).Content;
            }
            else return null;
        }

        public Effect GetEffect(string resourceString)
        {
            if (Effects.Count(res => res.ResourceString.Equals(resourceString)) == 1)
            {
                return Effects.First(res => res.ResourceString.Equals(resourceString)).Content;
            }
            else return null;
        }
    }
}
