using _ReplaceString_.Config;
using Microsoft.Xna.Framework;

namespace _ReplaceString_;
internal class UpdateSystem : ILoadable
{

    public void Load(Mod mod)
    {
        On.Terraria.Main.Update += Main_Update;
    }

    private void Main_Update(On.Terraria.Main.orig_Update orig, Terraria.Main self, GameTime gameTime)
    {
        UIFocusInputTextFieldReplaced.Check();
        orig(self, gameTime);
    }

    public void Unload()
    {

    }

}
