#region Using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RockRainZune.Core;

#endregion

namespace RockRainZune
{
    /// <summary>
    /// This is a game component thats represents the Instrucions Scene
    /// </summary>
    public class HelpScene : GameScene
    {
        public HelpScene(Game game, Texture2D textureBack)
            : base(game)
        {
            Components.Add(new ImageComponent(game, textureBack, 
                ImageComponent.DrawMode.Stretch));
        }
    }
}