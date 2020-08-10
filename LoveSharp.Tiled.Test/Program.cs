using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Love;
using Love.Tiled;

namespace Test
{
    class Program : Scene
    {
        Renderer renderer = new Renderer("./tmx/iso.tmx");
        //Renderer renderer = new Renderer("./tmx/hex.tmx");
        //Renderer renderer = new Renderer("./tmx/ortho.tmx");

        public static void Main(string[] args)
        {
            Boot.Init();
            Boot.Run(new Program());
        }

        public override void Draw()
        {
            renderer.Draw();
        }
    }
}
