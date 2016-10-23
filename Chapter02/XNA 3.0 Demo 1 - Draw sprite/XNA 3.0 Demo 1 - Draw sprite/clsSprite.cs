using Microsoft.Xna.Framework.Graphics;   //   for Texture2D
using Microsoft.Xna.Framework;  //  for Vector2

namespace XNADemo
{
    class clsSprite
    {
        public Texture2D texture { get; set; }      //  sprite texture, read-only property
        public Vector2 position { get; set; }  //  sprite position on screen
        public Vector2 size { get; set; }      //  sprite size in pixels

        public clsSprite (Texture2D newTexture, Vector2 newPosition, Vector2 newSize){
            texture = newTexture;
            position = newPosition;
            size = newSize;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }
    }
}
