using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlightSim
{
    public class Bullet
    {
        public Vector3 position;
        public Quaternion rotation;

        public Bullet(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;

        }

        public void Update()
        {

        }

    }

}
