using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlightSim
{
    public class Target
    {
        public BoundingSphere boundingSphere { get; private set; }
        Model model;

        public Target(Vector3 position, float radius, Model model)
        {
            this.model = model;
            boundingSphere = new BoundingSphere(position, radius);
        }

        public void Draw(DrawHelper drawHelper)
        {
            Matrix worldMatrix = Matrix.CreateScale(boundingSphere.Radius) * Matrix.CreateTranslation(boundingSphere.Center);
            drawHelper.Draw(model, worldMatrix);
        }
    }
}
