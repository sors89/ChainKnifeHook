using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;

namespace Sors
{
    [ApiVersion(2, 1)]
    public class ChainKnifeHook : TerrariaPlugin
    {
        public override string Author => "Sors";

        public override string Description => "";

        public override string Name => "ChainKnifeHook";

        public override Version Version => new(1, 0, 0);

        public ChainKnifeHook(Main game) : base(game)
        {
        }

        public override void Initialize()
        {
            Commands.ChatCommands.Add(new Command(
                permissions: new List<string> { },
                cmd: this.CKHook,
                "hook"));
        }

        private void CKHook(CommandArgs args)
        {
            int itemIndex = Item.NewItem(new EntitySource_DebugCommand(), args.Player.TPlayer.position, args.Player.TPlayer.Size, ItemID.ChainKnife);
            Item targetItem = Main.item[itemIndex];
            targetItem.playerIndexTheItemIsReservedFor = args.Player.Index;

            BitsByte flags_1, flags_2; flags_1 = flags_2 = new();
            flags_1[0] = flags_1[1] = flags_1[2] = flags_1[3] = false;
            flags_1[4] = flags_1[5] = flags_1[6] = flags_1[7] = true;

            flags_2[0] = flags_2[1] = flags_2[2] = flags_2[3] = flags_2[4] = false;
            flags_2[5] = true;

            byte[] tweak = new PacketFactory()
                .SetType((short)PacketTypes.TweakItem)
                .PackInt16((short)itemIndex)
                .PackByte(flags_1)
                .PackUInt16(600)
                .PackInt16(ProjectileID.LunarHookVortex)
                .PackSingle(20f)
                .PackByte(flags_2)
                .PackBool(true)
                .GetByteData();

            byte[] update = new PacketFactory()
                .SetType((short)PacketTypes.UpdateItemDrop)
                .PackInt16((short)itemIndex)
                .PackSingle(args.Player.TPlayer.position.X)
                .PackSingle(args.Player.TPlayer.position.Y)
                .PackSingle(0f)
                .PackSingle(0f)
                .PackInt16(1)
                .PackByte(0)
                .PackByte(0)
                .PackInt16((short)targetItem.netID)
                .GetByteData();

            byte[] owner = new PacketFactory()
                .SetType((short)PacketTypes.ItemOwner)
                .PackInt16((short)itemIndex)
                .PackByte((byte)args.Player.Index)
                .GetByteData();

            args.Player.SendRawData(update);
            args.Player.SendRawData(owner);
            args.Player.SendRawData(tweak);
        }
    }
}
