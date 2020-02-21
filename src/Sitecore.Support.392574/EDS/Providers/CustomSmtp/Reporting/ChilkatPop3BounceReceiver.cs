namespace Sitecore.Support.EDS.Providers.CustomSmtp.Reporting
{
    using System;
    using Sitecore.EDS.Core.Diagnostics;
    using Sitecore.EDS.Core.Net.Pop3;
    using Sitecore.EDS.Core.Reporting;
    using Sitecore.ExM.Framework.Diagnostics;

    public class ChilkatPop3BounceReceiver : Sitecore.EDS.Providers.CustomSmtp.Reporting.ChilkatPop3BounceReceiver
    {
        private readonly ILogger logger;
        public ChilkatPop3BounceReceiver(Pop3Settings settings, IBounceInspector inspector, IEnvironmentId environmentId) : base(settings, inspector, environmentId)
        {
        }

        public ChilkatPop3BounceReceiver(Pop3Settings settings, IBounceInspector inspector, IEnvironmentId environmentId, ILoggerFactory loggerFactory) : base(settings, inspector, environmentId, loggerFactory)
        {
            logger = loggerFactory.Logger;
        }

        public ChilkatPop3BounceReceiver(Pop3Settings settings, IBounceInspector inspector, IEnvironmentId environmentId, Sitecore.ExM.Framework.Diagnostics.ILogger logger) : base(settings, inspector, environmentId, logger)
        {
            this.logger = logger;
        }

        public override BounceStatus InspectEmail(IPop3Мail pop3Мail, IPop3Client pop3Client)
        {
            try
            {
                return base.InspectEmail(pop3Мail, pop3Client);
            }
            catch (Exception ex)
            {
                var messageSubject = pop3Мail.GetHeader("Subject") != null ? pop3Мail.GetHeader("Subject") : "[no subject]";
                var messageDate = pop3Мail.GetHeader("Date") != null ? pop3Мail.GetHeader("Date") : "[no date]";
                this.logger.TraceWarn("Bounce status of \"" + messageSubject + "\" message cannot be resolved due to " + ex.GetType().ToString() + ", skipping. Datetime: " + messageDate + ". UIDL: " + pop3Мail.Uidl);
                return BounceStatus.NotBounce;
            }
        }
    }
}