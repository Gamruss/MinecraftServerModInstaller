using System;

namespace MinecraftServerModInstaller
{
    class Program
    {
        static void Main(string[] args)
        {
            MinecraftInstaller installer = new MinecraftInstaller();
            installer.Start(args);
        }
    }
}
