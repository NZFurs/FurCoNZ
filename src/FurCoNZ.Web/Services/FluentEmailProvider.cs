using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using FluentEmail.Core;

using FurCoNZ.Web.Helpers;
using FurCoNZ.Web.Options;

namespace FurCoNZ.Web.Services
{
    public class FluentEmailProvider : IEmailProvider
    {
        private readonly IFluentEmail _email;
        private readonly ILogger _logger;
        private readonly FluentEmailProviderOptions _options;

        public FluentEmailProvider(IFluentEmail email, IOptions<FluentEmailProviderOptions> options, ILogger<SendGridEmailProvider> logger)
        {
            _email = email;
            _logger = logger;
            _options = options.Value;
        }

        public async Task SendEmailAsync(MailAddressCollection to, string subject, string htmlBody, IEnumerable<Attachment> attachments = null, CancellationToken cancellationToken = default)
        {
            var message = _email
                .To(to.ToFluentEmailAddresses())
                .SetFrom(_options.FromAddress, _options.FromName)
                .Subject(subject)
                .Body(htmlBody, true);

            if (attachments != null && attachments.Any())
            {
                message.Attach(attachments.ToFluentEmailAttachments().ToList());
            }

            try
            {
                var response = await _email.SendAsync(cancellationToken);
                if(!response.Successful)
                {
                    _logger.LogError($"Failed to send email to {to}. Email sender response:");
                    foreach (var errorMessage in response.ErrorMessages)
                    {
                        _logger.LogError(errorMessage);
                    }
                    
                }
            }
            catch (Exception ex) {
                _logger.LogError(ex, $"Failed to send email to {to}.");
            }
        }
    }
}
