using MelonLoader;

namespace CustomLoadingScreens.Utilities
{
    public class Logger
    {
        private readonly MelonLogger.Instance _logger;

        public Logger(string className)
        {
            _logger = new MelonLogger.Instance(className);
        }

        public void Msg(object message, bool verbose = true)
        {
            if (verbose && !ModSettings.VerboseLogging) return;
            _logger.Msg(message);
        }

        public void Warning(object message)
        {
            _logger.Msg(ConsoleColor.Yellow, "Warning: " + message);
        }

        public void Error(string message)
        {
            _logger.Msg(ConsoleColor.Red, "ERROR: " + message);
        }
    }
}
