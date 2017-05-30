using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Bomberman_client.GameClasses
{
    [Serializable]
    public class Cell
    {
        private Point location;
        public int X
        {
            set
            {
                location.X = value;
            }
            get
            {
                return location.X;
            }
        }
        public int Y
        {
            set
            {
                location.Y = value;
            }
            get
            {
                return location.Y;
            }
        }
        //public Image texture { get; set; }
        public byte[] Serialize(BinaryFormatter serializer)
        {
            MemoryStream data = new MemoryStream();
            serializer.Serialize(data, this);

            return data.ToArray();
        }
        public Cell(Point location)
        {
            //this.texture = texture;

            this.location = location;
        }
    }
}
