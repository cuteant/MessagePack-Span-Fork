using System;
using Microsoft.Extensions.Logging;

namespace ServiceStack.Text
{
    public class Tracer
    {
        public static ITracer Instance = new MsLoggingTracer();

        public class NullTracer : ITracer
        {
            public void WriteDebug(string error) { }

            public void WriteDebug(string format, params object[] args) { }

            public void WriteWarning(string warning) { }

            public void WriteWarning(string format, params object[] args) { }

            public void WriteError(Exception ex) { }

            public void WriteError(string error) { }

            public void WriteError(string format, params object[] args) { }

        }
        internal sealed class MsLoggingTracer : ITracer
        {
            private readonly ILogger _logger = PclExport.Instance.Logger;

            public void WriteDebug(string error)
            {
                if (_logger.IsDebugLevelEnabled()) { _logger.LogDebug(error); }
            }

            public void WriteDebug(string format, params object[] args)
            {
                if (_logger.IsDebugLevelEnabled()) { _logger.LogDebug(format, args); }
            }

            public void WriteWarning(string warning)
            {
                if (_logger.IsWarningLevelEnabled()) { _logger.LogWarning(warning); }
            }

            public void WriteWarning(string format, params object[] args)
            {
                if (_logger.IsWarningLevelEnabled()) { _logger.LogWarning(format, args); }
            }

            public void WriteError(Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

            public void WriteError(string error)
            {
                _logger.LogError(error);
            }

            public void WriteError(string format, params object[] args)
            {
                _logger.LogError(format, args);
            }

        }

        //public class ConsoleTracer : ITracer
        //{
        //    public void WriteDebug(string error)
        //    {
        //        PclExport.Instance.WriteLine(error);
        //    }

        //    public void WriteDebug(string format, params object[] args)
        //    {
        //        PclExport.Instance.WriteLine(format, args);
        //    }

        //    public void WriteWarning(string warning)
        //    {
        //        PclExport.Instance.WriteLine(warning);
        //    }

        //    public void WriteWarning(string format, params object[] args)
        //    {
        //        PclExport.Instance.WriteLine(format, args);
        //    }

        //    public void WriteError(Exception ex)
        //    {
        //        PclExport.Instance.WriteLine(ex.ToString());
        //    }

        //    public void WriteError(string error)
        //    {
        //        PclExport.Instance.WriteLine(error);
        //    }

        //    public void WriteError(string format, params object[] args)
        //    {
        //        PclExport.Instance.WriteLine(format, args);
        //    }
        //}
    }

    public static class TracerExceptions
    {
        public static T Trace<T>(this T ex) where T : Exception
        {
            Tracer.Instance.WriteError(ex);
            return ex;
        }
    }
}