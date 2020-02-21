using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.EDS.Core.Net.Pop3;
using Sitecore.EDS.Core.Reporting;
using Sitecore.EDS.Providers.CustomSmtp.Reporting;
using Sitecore.ExM.Framework.Diagnostics;
using Sitecore.Modules.EmailCampaign.Services;

namespace Sitecore.Support.Modules.EmailCampaign.Core.MessageTransfer
{
    public class ManagerRootsPop3ReceiversCollection : IPop3ReceiversCollection
    {

        private readonly ILogger logger;

        private readonly IManagerRootService _managerRootService;

        private readonly IBounceInspector inspector;

        private readonly IEnvironmentId environmentId;

        public ManagerRootsPop3ReceiversCollection([NotNull] IBounceInspector inspector, [NotNull] IEnvironmentId environmentId, [NotNull] ILogger logger)
          : this(ServiceLocator.ServiceProvider.GetService<IManagerRootService>(), inspector, environmentId, logger)
        {
            Assert.ArgumentNotNull(inspector, "inspector");
            Assert.ArgumentNotNull(environmentId, "environmentId");
            Assert.ArgumentNotNull(logger, "logger");
        }

        public ManagerRootsPop3ReceiversCollection([NotNull] IManagerRootService managerRootService, [NotNull] IBounceInspector inspector, [NotNull] IEnvironmentId environmentId, [NotNull] ILogger logger)
        {
            Assert.ArgumentNotNull(managerRootService, nameof(managerRootService));
            Assert.ArgumentNotNull(inspector, "inspector");
            Assert.ArgumentNotNull(environmentId, "environmentId");
            Assert.ArgumentNotNull(logger, "logger");

            _managerRootService = managerRootService;
            this.inspector = inspector;
            this.environmentId = environmentId;
            this.logger = logger;
        }

        public IEnumerable<IPop3BounceReceiver> Receivers()
        {
            var receivers = new List<ChilkatPop3BounceReceiver>();
            foreach (var managerRoot in _managerRootService.GetManagerRoots())
            {
                if (managerRoot.Settings.GatherNotifications)
                {
                    var settings = new Pop3Settings
                    {
                        Password = managerRoot.Settings.POP3Password,
                        Port = managerRoot.Settings.POP3Port,
                        Server = managerRoot.Settings.POP3Server,
                        UseSsl = managerRoot.Settings.POP3SSL,
                        UserName = managerRoot.Settings.POP3UserName,
                        StartTls = !managerRoot.Settings.POP3SSL
                    };

                    try
                    {
                        var receiver = this.CreateReceiver(settings);
                        receivers.Add(receiver);
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogError(ex);
                    }
                }
            }

            return receivers;
        }

        private ChilkatPop3BounceReceiver CreateReceiver(Pop3Settings settings)
        {
            Assert.ArgumentNotNullOrEmpty(settings.Server, "settings.Server");
            Assert.ArgumentCondition(settings.Port > 0, "settings.Port", "Missing Port number.");

            return new Sitecore.Support.EDS.Providers.CustomSmtp.Reporting.ChilkatPop3BounceReceiver(settings, this.inspector, this.environmentId, this.logger);
        }
    }
}