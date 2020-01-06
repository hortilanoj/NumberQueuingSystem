
namespace NumberQueuingSystem.BOL.Task
{
    public class objTask
    {
        #region Enums
        public enum MessageType : ushort
        {
            None = 0,
            RefreshTerminal = 1,
            ChangeMedia = 2,
            LoadTerminals = 3,
            NextQueue = 4,
            Recall = 5,
            Pause = 6,
            Play = 7,
            RefreshFrontDesk = 8,
            Mute = 9,
            Unmute = 10,
            DisposeTerminals =11,
            QueueForward = 12,
            Unbind = 13,
            RefreshClientTerminal = 14,
            CurrentIsSet = 15
        }
        #endregion

        public long ? id { get; set; }
        public MessageType ? type { get; set; }
        public long ? terminal_id { get; set; }
    }
}
