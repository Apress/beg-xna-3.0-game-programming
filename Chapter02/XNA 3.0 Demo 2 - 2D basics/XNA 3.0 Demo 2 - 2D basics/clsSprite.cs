using Microsoft.Xna.Framework.Graphics;   //   for Texture2D
using Microsoft.Xna.Framework;  //  for Vector2

namespace XNADemo
{
    class clsSprite
    {
        public Texture2D texture { get; set; } //  sprite texture, read-only property
        public Vector2 position { get; set; }  //  sprite position on screen
        public Vector2 size { get; set; }      //  sprite size in pixels
        public Vector2 velocity { get; set; }  //  sprite velocity
        private Vector2 screenSize { get; set; } //  screen size

        public Vector2 center{ get{ return position + (size/2);}  } //  sprite center
        public float radius { get { return size.X / 2; } } //  sprite radius

        public clsSprite (Texture2D newTexture, Vector2 newPosition, Vector2 newSize, int ScreenWidth, int ScreenHeight){
            texture = newTexture;
            position = newPosition;
            size = newSize;
            screenSize = new Vector2(ScreenWidth, ScreenHeight);
        }

        public bool Collides(clsSprite otherSprite)
        {
            // check if two sprites intersect
            return (this.position.X + this.size.X > otherSprite.position.X && 
                    this.position.X < otherSprite.position.X + otherSprite.size.X &&
                    this.position.Y + this.size.Y > otherSprite.position.Y &&
                    this.position.Y < otherSprite.position.Y + otherSprite.size.Y);
        }

        public bool CircleCollides(clsSprite otherSprite)
        {    //  Check if two circle sprites collided
            return (Vector2.Distance(this.center, otherSprite.center) <
                this.radius + otherSprite.radius);
        }

        public void Move()
        {
            //  if we´ll move out of the screen, invert velocity

            //  checking right boundary
            if (position.X + size.X + velocity.X > screenSize.X)
                velocity = new Vector2(-velocity.X, velocity.Y);
            //  checking bottom boundary
            if (position.Y + size.Y + velocity.Y > screenSize.Y)
                velocity = new Vector2(velocity.X, -velocity.Y);
            //  checking left boundary
            if (position.X + velocity.X < 0)
                velocity = new Vector2(-velocity.X, velocity.Y);
            //  checking bottom boundary
            if (position.Y + velocity.Y < 0)
                velocity = new Vector2(velocity.X, -velocity.Y);

            //  since we adjusted the velocity, just add it to the current position
            position += velocity;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }
    }
}
