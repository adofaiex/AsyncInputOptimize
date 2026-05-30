using ModsTagLib.Win32;

namespace AsyncInput
{
    public struct AsyncKeyEvent
    {
        public ulong time;
        public VirtualKeys key;
        public bool state;
        public ushort unuse;
    }
}
