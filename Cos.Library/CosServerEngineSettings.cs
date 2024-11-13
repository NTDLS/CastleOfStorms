namespace Cos.Library
{
    public class CosServerEngineSettings
    {
        /// <summary>
        /// The listen port for data exchange.
        /// </summary>
        public int DataPort { get; set; }

        /// <summary>
        /// The amount of time in milliseconds between updates of player absolute state to the server.
        /// </summary>
        public int PlayerAbsoluteStateDelayMs { get; set; }

        /// <summary>
        /// Causes the server to write super-verbose information about almost every internal operation.
        /// </summary>
        public bool WriteTraceData { get; set; }

        /// <summary>
        /// If true, text logs will be flushed at every write. This ensures that the log file is always up-to-date on disk.
        /// </summary>
        public bool FlushLog { get; set; }

        /// <summary>
        /// The directory where text and performance logs are stores.
        /// </summary>
        public string LogDirectory
        {
            get => logDirectory;
            set => logDirectory = value.TrimEnd(['/', '\\']).Trim();
        }
        private string logDirectory = string.Empty;

        /// <summary>
        /// The number of seconds that a connection must be idle before it is automatically closed.
        /// </summary>
        public int MaxIdleConnectionSeconds { get; set; }
    }
}
